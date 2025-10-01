using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.Entities;

namespace ProductManagement.Services.Abstract;

// Interfaz que define los métodos para manejar productos
public interface IProductService
{
  // Obtiene todos los productos con sus imágenes asociadas
  Task<IEnumerable<ProductResponseDto>> GetAllProducts();
  // Busca un producto específico por su ID
  Task<ProductResponseDto?> GetProductById(Guid id);
  // Crea un nuevo producto
  Task<ProductResponseDto> CreateProduct(AddProductDto product);
  // Actualiza un producto existente
  Task<ProductResponseDto?> UpdateProduct(Guid id, UpdateProductDto updatedProduct);
  // Elimina un producto de la base de datos
  Task<bool> DeleteProduct(Guid id);
}

// Servicio que maneja la lógica de negocio para los productos
public class ProductService : IProductService
{
    // Contexto de la base de datos para acceder a las tablas
    private readonly ApplicationDbContext _dbContext;

    // Constructor que recibe el contexto de base de datos
    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Método que obtiene todos los productos con sus imágenes
    public async Task<IEnumerable<ProductResponseDto>> GetAllProducts()
    {
        // Realiza un JOIN entre productos e imágenes para obtener datos completos
        return await _dbContext.Products
            .GroupJoin(_dbContext.Images,
                product => product.ImageId, // Campo de relación en Product
                image => image.Id.ToString(), // Campo de relación en Image
                (product, images) => new { product, images })
            .SelectMany(x => x.images.DefaultIfEmpty(), // Permite productos sin imagen
                (x, image) => new ProductResponseDto
                {
                    // Mapea los datos del producto
                    Id = x.product.Id,
                    Name = x.product.Name,
                    Description = x.product.Description,
                    Price = x.product.Price,
                    Stock = x.product.Stock,
                    ImageId = x.product.ImageId,
                    CreatedAt = x.product.CreatedAt,
                    UpdatedAt = x.product.UpdatedAt,
                    IsActive = x.product.IsActive,
                    // Incluye la imagen si existe, sino null
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

    // Método que busca un producto específico por ID
    public async Task<ProductResponseDto?> GetProductById(Guid id)
    {
        // Similar a GetAllProducts pero filtra por ID específico
        return await _dbContext.Products
            .Where(p => p.Id == id) // Filtra por el ID del producto
            .GroupJoin(_dbContext.Images,
                product => product.ImageId,
                image => image.Id.ToString(),
                (product, images) => new { product, images })
            .SelectMany(x => x.images.DefaultIfEmpty(),
                (x, image) => new ProductResponseDto
                {
                    // Mapea los datos del producto
                    Id = x.product.Id,
                    Name = x.product.Name,
                    Description = x.product.Description,
                    Price = x.product.Price,
                    Stock = x.product.Stock,
                    ImageId = x.product.ImageId,
                    CreatedAt = x.product.CreatedAt,
                    UpdatedAt = x.product.UpdatedAt,
                    IsActive = x.product.IsActive,
                    // Incluye la imagen si existe, sino null
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
            .FirstOrDefaultAsync(); // Retorna el primer resultado o null
    }

    // Método que crea un nuevo producto
    public async Task<ProductResponseDto> CreateProduct(AddProductDto addProductDto)
    {
        // Validación: el precio debe ser mayor a cero
        if (addProductDto.Price <= 0)
        {
            throw new Exception("Price must be greater than zero");
        }
        
        // Validación: el stock no puede ser negativo
        if (addProductDto.Stock < 0)
        {
            throw new Exception("Stock cannot be negative");
        }
        
        // Crea una nueva entidad Product con los datos recibidos
        var productEntity = new Product()
        {
            Name = addProductDto.Name,
            Description = addProductDto.Description,
            Price = addProductDto.Price,
            Stock = addProductDto.Stock,
            IsActive = addProductDto.IsActive,
            ImageId = addProductDto.ImageId,
            CreatedAt = DateTime.UtcNow, // Fecha de creación actual
            UpdatedAt = DateTime.UtcNow  // Fecha de actualización actual
        };

        // Agrega el producto al contexto y guarda en la base de datos
        _dbContext.Products.Add(productEntity);
        await _dbContext.SaveChangesAsync();

        // Retorna el producto creado con todos sus datos (incluye imagen si la tiene)
        return await GetProductById(productEntity.Id) ??
               throw new InvalidOperationException("Failed to create product");
    }

    // Método que actualiza un producto existente
    public async Task<ProductResponseDto?> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
    {
        // Busca el producto en la base de datos por ID
        var product = await _dbContext.Products.FindAsync(id);
        if (product is null)
            return null; // Si no existe, retorna null

        // Actualiza todos los campos del producto
        product.Name = updateProductDto.Name;
        product.Description = updateProductDto.Description;
        product.Price = updateProductDto.Price;
        product.Stock = updateProductDto.Stock;
        product.IsActive = updateProductDto.IsActive;
        product.ImageId = updateProductDto.ImageId;
        product.UpdatedAt = DateTime.UtcNow; // Actualiza la fecha de modificación

        // Guarda los cambios en la base de datos
        await _dbContext.SaveChangesAsync();
        // Retorna el producto actualizado con todos sus datos
        return await GetProductById(id);
    }

    // Método que elimina un producto de la base de datos
    public async Task<bool> DeleteProduct(Guid id)
    {
        // Busca el producto por ID
        var product = await _dbContext.Products.FindAsync(id);
        if (product is null)
            return false; // Si no existe, retorna false

        // Elimina el producto de la base de datos
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
        return true; // Retorna true si se eliminó correctamente
    }
}
