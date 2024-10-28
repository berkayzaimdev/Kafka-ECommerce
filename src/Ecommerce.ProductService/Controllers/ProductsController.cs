using Ecommerce.Model;
using Ecommerce.ProductService.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ProductService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(ProductDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ProductModel>> GetProducts()
    {
        return await dbContext.Products.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ProductModel> GetProduct(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }
}
