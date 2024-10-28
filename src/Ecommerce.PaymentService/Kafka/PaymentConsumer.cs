using Confluent.Kafka;
using Ecommerce.Common;
using Ecommerce.Model;
using System.Text.Json;

namespace Ecommerce.PaymentService.Kafka;

public class PaymentConsumer(IKafkaProducer producer) : KafkaConsumer(topics)
{
    private static readonly string[] topics = { "products-reserved" };

    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);
        switch (consumeResult.Topic)
        {
            case "products-reserved":
                await HandleProductsReserved(consumeResult.Message.Value);
                break;
        }
    }

    private async Task HandleProductsReserved(string message)
    {
        var orderMessage = JsonSerializer.Deserialize<OrderMessage>(message);

        var isPaymentProcessed = ProcessPayment(orderMessage);

        if (isPaymentProcessed)
        {
            await producer.ProduceAsync("payment-processed", orderMessage);
        }
        else
        {
            await producer.ProduceAsync("payment-failed", orderMessage);
        }
    }

    private bool ProcessPayment(OrderMessage orderMessage)
    {
        return true;
    }
}
