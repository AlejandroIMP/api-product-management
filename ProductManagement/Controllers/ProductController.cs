using Microsoft.AspNetCore.Mvc;
using ProductManagement.Models;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AddProductDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(AddProductDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductById(id);
            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AddProductDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
        {
            try
            {
                var product = await _productService.CreateProduct(addProductDto);
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest("Error creating product");
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(AddProductDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
        {
            var product = await _productService.UpdateProduct(id, updateProductDto);
            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var success = await _productService.DeleteProduct(id);
            if (!success) return NotFound();
            return Ok();
        }
    }
}