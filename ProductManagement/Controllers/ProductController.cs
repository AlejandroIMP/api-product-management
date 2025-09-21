using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.Entities;

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
        
        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetProductById(Guid id)
        {
            var product = _dbContext.Products.Find(id);

            if (product is null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        [HttpPost]
        public IActionResult AddProduct(AddProductDto addProductDto)
        {
            var productEntity = new Product()
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                Price = addProductDto.Price,
                Stock = addProductDto.Stock,
                IsActive = addProductDto.IsActive,
            };

            try
            {
                _dbContext.Products.Add(productEntity);
                _dbContext.SaveChanges();
                return Ok(productEntity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
