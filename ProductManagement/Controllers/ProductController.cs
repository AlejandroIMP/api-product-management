using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var allProducts = _dbContext.Products.ToList();
            return Ok(allProducts);
        }
    }
}
