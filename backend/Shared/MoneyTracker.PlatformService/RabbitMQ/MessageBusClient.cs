
using System.Text;
using System.Text.Json;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MoneyTracker.PlatformService.RabbitMQ;
public class MessageBusClient : IMessageBusClient
{
    private IConnection _connection;
    private IChannel _channel;

    public MessageBusClient(string connectionString)
    {
        InitializeAsync(connectionString).Wait();
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

            _connection.ConnectionShutdownAsync += connection_ConnectionShutdown;

            Console.WriteLine("Connected to message bus");
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

    public async Task PublishEvent(EventUpdate eventToSend,
        CancellationToken cancellationToken)
    {
        var message = JsonSerializer.Serialize(eventToSend);

        if (_connection.IsOpen)
        {
            await SendMessage(message, cancellationToken);
        }
        else
        {
            Console.WriteLine("Connection closed - failed to send");
        }
    }

    private async Task SendMessage(string message, CancellationToken cancellationToken)
    {
        var body = Encoding.UTF8.GetBytes(message);

        if (_channel.IsOpen)
        {
            await _channel.BasicPublishAsync("Temp", "", body, cancellationToken);
            Console.WriteLine("Message sent");
        }
        else
        {
            Console.WriteLine($"Channel closed - failed to send");
        }
    }

    private async Task connection_ConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        await Task.CompletedTask;
        Console.WriteLine($"Message bus is now closed due to: {reason}");
    }
}
