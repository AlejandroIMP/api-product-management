using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.Entities;


namespace ProductManagement.Services;

public interface IProductService
{
  Task<IEnumerable<ProductResponseDto>> GetAllProducts();
  Task<ProductResponseDto?> GetProductById(Guid id);
  Task<ProductResponseDto> CreateProduct(AddProductDto product);
  Task<ProductResponseDto?> UpdateProduct(Guid id, UpdateProductDto updatedProduct);
  Task<bool> DeleteProduct(Guid id);
}

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProducts()
    {
        return await _dbContext.Products
            .GroupJoin(_dbContext.Images,
                product => product.ImageId,
                image => image.Id.ToString(),
                (product, images) => new { product, images })
            .SelectMany(x => x.images.DefaultIfEmpty(),
                (x, image) => new ProductResponseDto
                {
                    Id = x.product.Id,
                    Name = x.product.Name,
                    Description = x.product.Description,
                    Price = x.product.Price,
                    Stock = x.product.Stock,
                    ImageId = x.product.ImageId,
                    CreatedAt = x.product.CreatedAt,
                    UpdatedAt = x.product.UpdatedAt,
                    IsActive = x.product.IsActive,
                    Image = image != null
                        ? new ImageDto
                        {
                            Id = image.Id,
                            PublicId = image.PublicId,
                            Url = image.Url,
                            MetadatosJson = image.MetadatosJson,
                            CreatedAt = image.CreatedAt,
                            UpdatedAt = image.UpdatedAt
                        }
                        : null
                })
            .ToListAsync();
    }

    public async Task<ProductResponseDto?> GetProductById(Guid id)
    {
        return await _dbContext.Products
            .Where(p => p.Id == id)
            .GroupJoin(_dbContext.Images,
                product => product.ImageId,
                image => image.Id.ToString(),
                (product, images) => new { product, images })
            .SelectMany(x => x.images.DefaultIfEmpty(),
                (x, image) => new ProductResponseDto
                {
                    Id = x.product.Id,
                    Name = x.product.Name,
                    Description = x.product.Description,
                    Price = x.product.Price,
                    Stock = x.product.Stock,
                    ImageId = x.product.ImageId,
                    CreatedAt = x.product.CreatedAt,
                    UpdatedAt = x.product.UpdatedAt,
                    IsActive = x.product.IsActive,
                    Image = image != null
                        ? new ImageDto
                        {
                            Id = image.Id,
                            PublicId = image.PublicId,
                            Url = image.Url,
                            MetadatosJson = image.MetadatosJson,
                            CreatedAt = image.CreatedAt,
                            UpdatedAt = image.UpdatedAt
                        }
                        : null
                })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductResponseDto> CreateProduct(AddProductDto addProductDto)
    {
        var productEntity = new Product()
        {
            Name = addProductDto.Name,
            Description = addProductDto.Description,
            Price = addProductDto.Price,
            Stock = addProductDto.Stock,
            IsActive = addProductDto.IsActive,
            ImageId = addProductDto.ImageId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Products.Add(productEntity);
        await _dbContext.SaveChangesAsync();

        return await GetProductById(productEntity.Id) ??
               throw new InvalidOperationException("Failed to create product");
    }

    public async Task<ProductResponseDto?> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product is null)
            return null;

        product.Name = updateProductDto.Name;
        product.Description = updateProductDto.Description;
        product.Price = updateProductDto.Price;
        product.Stock = updateProductDto.Stock;
        product.IsActive = updateProductDto.IsActive;
        product.ImageId = updateProductDto.ImageId;
        product.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return await GetProductById(id);
    }

    public async Task<bool> DeleteProduct(Guid id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product is null)
            return false;

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
