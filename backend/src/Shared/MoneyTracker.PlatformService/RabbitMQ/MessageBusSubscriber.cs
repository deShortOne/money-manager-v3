using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MoneyTracker.PlatformService.RabbitMQ;
public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueName;
    private readonly IEventProcessor _eventProcessor;

    private MessageBusSubscriber(IConnection connection,
        IChannel channel,
        string queueName,
        IEventProcessor eventProcessor)
    {
        _connection = connection;
        _channel = channel;
        _queueName = queueName;
        _eventProcessor = eventProcessor;
    }

    public static async Task<MessageBusSubscriber> InitializeAsync(string connectionString, IEventProcessor eventProcessor)
    {
        try
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            var connection = await connectionFactory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync("Temp", ExchangeType.Fanout);

            var queueName = (await channel.QueueDeclareAsync()).QueueName;
            await channel.QueueBindAsync(queueName, exchange: "Temp", routingKey: "");

            connection.ConnectionShutdownAsync += connection_ConnectionShutdown;

            Console.WriteLine("Listening to message bus");

            return new MessageBusSubscriber(connection, channel, queueName, eventProcessor);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to message bus: {ex.Message}");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel.IsOpen)
        {
            await _channel.DisposeAsync();
        }
        if (_connection.IsOpen)
        {
            await _connection.DisposeAsync();
        }

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += (ModuleHandle, ea) =>
        {
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            var eventType = JsonSerializer.Deserialize<EventUpdate>(notificationMessage);

            if (eventType == null)
                throw new InvalidOperationException("Event passed is not correct object");

            _eventProcessor.ProcessEvent(eventType, cancellationToken);

            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(_queueName,
            true,
            consumer,
            cancellationToken);
    }

    private static async Task connection_ConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        await Task.CompletedTask;
        Console.WriteLine($"Message bus is now closed due to: {reason}");
    }
}
