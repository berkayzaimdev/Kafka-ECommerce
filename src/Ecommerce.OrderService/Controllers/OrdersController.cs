using Ecommerce.Model;
using Ecommerce.OrderService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.OrderService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrdersController(OrderDbContext dbContext) : ControllerBase
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
        return order;
    }
}
