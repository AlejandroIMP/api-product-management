namespace ProductManagement.Models;

public class ImageDto
{
    public Guid Id { get; set; }
    public string PublicId { get; set; } = "";
    public string Url { get; set; } = "";
    public string MetadatosJson { get; set; } = "";
    public ImageEnum MetadataSatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}