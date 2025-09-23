namespace ProductManagement.Models;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool? IsActive { get; set; }
    
    public ImageDto? Image { get; set; }
}