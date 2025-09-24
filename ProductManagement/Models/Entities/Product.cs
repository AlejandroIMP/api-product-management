using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models.Entities;

public class Product
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public required decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
    public required int Stock { get; set; }
    [StringLength(100)]
    public string? ImageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool? IsActive { get; set; }
}