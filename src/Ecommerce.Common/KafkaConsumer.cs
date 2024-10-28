using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Ecommerce.Common;

public class KafkaConsumer
    (IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer(string[] topics)
    {
        var config = new ConsumerConfig
        {
            GroupId = "ecommerce-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(topics);
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            await HandleConsumeAsync(stoppingToken);
        }, stoppingToken);
    }

    public async Task HandleConsumeAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(stoppingToken);
            await ConsumeAsync(consumeResult);
        }

        _consumer.Close();
    }

    protected virtual Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        return Task.CompletedTask;
    }
}
