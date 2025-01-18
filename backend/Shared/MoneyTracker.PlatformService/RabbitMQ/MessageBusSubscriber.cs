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
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;
    private readonly IEventProcessor _eventProcessor;

    public MessageBusSubscriber(string connectionString,
        IEventProcessor eventProcessor)
    {
        _eventProcessor = eventProcessor;
        InitializeAsync(connectionString).Wait();
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

            _eventProcessor.ProcessEvent(eventType);

            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(_queueName,
            true,
            consumer,
            cancellationToken);
    }

    private async Task InitializeAsync(string connectionString)
    {
        try
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            _connection = await connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("Temp", ExchangeType.Fanout);

            _queueName = (await _channel.QueueDeclareAsync()).QueueName;
            await _channel.QueueBindAsync(_queueName, exchange: "Temp", routingKey: "");

            _connection.ConnectionShutdownAsync += connection_ConnectionShutdown;

            Console.WriteLine("Listening to message bus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to message bus: {ex.Message}");
            throw;
        }
    }

    private async Task connection_ConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        await Task.CompletedTask;
        Console.WriteLine($"Message bus is now closed due to: {reason}");
    }
}
