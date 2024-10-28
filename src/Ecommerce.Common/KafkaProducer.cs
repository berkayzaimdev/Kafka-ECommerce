using Confluent.Kafka;
using System.Text.Json;

namespace Ecommerce.Common;
public interface IKafkaProducer
{
    Task ProduceAsync(string topic, object message);
}

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer; // produces message

    public KafkaProducer()
    {
        var config = new ConsumerConfig
        {
            GroupId = "order-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, object message)
    {
        var kafkaMessage = new Message<string, string>() { Value = JsonSerializer.Serialize(message) };
        await _producer.ProduceAsync(topic, kafkaMessage);
    }
}
