using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models.Entities;

public class Image
{
    public Guid Id { get; set; }
    
    [StringLength(255)]
    public required string PublicId { get; set; }
    
    [StringLength(255)]
    public required string Url { get; set; }
    
    public required string MetadatosJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}