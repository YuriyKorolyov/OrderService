using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.WebApi.Events;

public sealed class KafkaOrderEventProducer : IOrderEventProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaOrderEventProducer(IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"];
        if (string.IsNullOrWhiteSpace(bootstrapServers))
        {
            throw new InvalidOperationException(
                "Kafka bootstrap servers are not configured. " +
                "Please set 'Kafka:BootstrapServers' in configuration.");
        }

        _topic = configuration["Kafka:Topics:OrderCreated"] ?? "order-created";

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task PublishOrderCreatedAsync(Order order, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(new
        {
            order.Id,
            order.ProductId,
            order.Amount,
            order.EmailClient,
            order.Price,
            order.PhoneNumber,
            order.CreatedAt,
            order.Status
        });

        var message = new Message<string, string>
        {
            Key = order.Id.ToString(),
            Value = payload
        };

        // Confluent.Kafka doesn't support CancellationToken directly here,
        // but we can still honor cancellation between calls.
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await _producer.ProduceAsync(_topic, message);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}

