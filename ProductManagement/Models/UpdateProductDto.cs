namespace ProductManagement.Models;

public class UpdateProductDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool? IsActive { get; set; }
}