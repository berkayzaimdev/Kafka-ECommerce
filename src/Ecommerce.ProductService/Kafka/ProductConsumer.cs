using Confluent.Kafka;
using Ecommerce.Common;
using Ecommerce.Model;
using Ecommerce.ProductService.Data;
using System.Text.Json;

namespace Ecommerce.ProductService.Kafka;

public class ProductConsumer(IServiceProvider serviceProvider, IKafkaProducer producer) : KafkaConsumer(topics)
{
    private static readonly string[] topics = ["order-created"];

    private ProductDbContext GetDbContext()
    {
        var scope = serviceProvider.CreateScope();

        return scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    }

    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);

        switch (consumeResult.Topic)
        {
            case "order-created":
                await HandleOrderCreatedAsync(consumeResult.Message.Value);
                break;
        }
    }

    public async Task HandleOrderCreatedAsync(string message)
    {
        var orderMessage = JsonSerializer.Deserialize<OrderMessage>(message);
        var isReserved = await ReserveProducts(orderMessage);

        if (isReserved)
        {
            await producer.ProduceAsync("products-reserved", orderMessage);
        }

        else
        {
            await producer.ProduceAsync("products-reservation-failed", orderMessage);
        }
    }

    public async Task<bool> ReserveProducts(OrderMessage orderMessage)
    {
        using var dbContext = GetDbContext();
        var product = await dbContext.Products.FindAsync(orderMessage.ProductId);

        if (product == null && product.Quantity >= orderMessage.Quantity) 
        {
            product.Quantity -= orderMessage.Quantity;
            await dbContext.SaveChangesAsync();
            return true;
        }


        return false;
    }
}
