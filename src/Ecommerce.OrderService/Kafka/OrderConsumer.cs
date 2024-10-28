using Confluent.Kafka;
using Ecommerce.Common;
using Ecommerce.Model;
using Ecommerce.OrderService.Data;
using System.Text.Json;

namespace Ecommerce.OrderService.Kafka;

public class OrderConsumer(IServiceProvider serviceProvider, IKafkaProducer producer) : KafkaConsumer(topics)
{
    private static readonly string[] topics = { "payment-processed", "products-reservation-failed", "products-reservation-canceled" };

    private OrderDbContext GetDbContext()
    {
        var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        return dbContext;
    }

    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);
        switch (consumeResult.Topic)
        {
            case "payment-processed":
                await HandleConfirmOrder(consumeResult.Message.Value);
                break;

            case "products-reservation-failed":
                await HandleCancelOrder(consumeResult.Message.Value);
                break;

            case "products-reservation-canceled":
                await HandleCancelOrder(consumeResult.Message.Value);
                break;
        }
    }

    private async Task HandleConfirmOrder(string message)
    {
        var orderMessage = JsonSerializer.Deserialize<OrderMessage>(message);
        using var dbContext = GetDbContext();

        var order = await dbContext.Orders.FindAsync(orderMessage.OrderId);

        if (order is not null)
        {
            order.Status = "Confirm";
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task HandleCancelOrder(string message)
    {
        var orderMessage = JsonSerializer.Deserialize<OrderMessage>(message);
        using var dbContext = GetDbContext();

        var order = await dbContext.Orders.FindAsync(orderMessage.OrderId);

        if (order is not null)
        {
            order.Status = "Cancel";
            await dbContext.SaveChangesAsync();
        }
    }
}
