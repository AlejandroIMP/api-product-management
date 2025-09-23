namespace ProductManagement.Models;

public class AddProductDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int Stock { get; set; }
    public bool? IsActive { get; set; }
    public string? ImageId { get; set; }
}
