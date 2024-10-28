using Confluent.Kafka;

namespace Ecommerce.OrderService.Kafka;

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

    public async Task ProduceAsync(string topic, Message<string, string> message)
    {
        await _producer.ProduceAsync(topic, message);
    }
}
