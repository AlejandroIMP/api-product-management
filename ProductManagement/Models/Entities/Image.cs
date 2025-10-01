using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagement.Models.Entities;

public class Image
{
    public Guid Id { get; set; }
    
    [StringLength(255)]
    public required string PublicId { get; set; }
    
    [StringLength(255)]
    public required string Url { get; set; }
    
    // Allow large metadata JSON from AI providers
    [Column(TypeName = "nvarchar(max)")]
    public required string MetadatosJson { get; set; }
    [StringLength(100)]
    public required string MetadataStatus { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}