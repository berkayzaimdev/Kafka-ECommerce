using Ecommerce.Common;
using Ecommerce.Model;
using Ecommerce.OrderService.Data;
using Ecommerce.OrderService.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ecommerce.OrderService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrdersController(OrderDbContext dbContext, IKafkaProducer producer) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<OrderModel>> GetOrders()
    {
        return await dbContext.Orders.ToListAsync();
    }

    [HttpPost]
    public async Task<OrderModel> CreateOrder(OrderModel order)
    {
        order.OrderDate = DateTime.Now;
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var orderMessage = new OrderMessage
        {
            OrderId = order.Id,
            ProductId = order.ProductId,
            Quantity = order.Quantity
        };

        await producer.ProduceAsync("order-created", orderMessage);

        return order;
    }
}
