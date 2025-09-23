namespace ProductManagement.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Models.Entities;
using Data;
using System.Text.Json;

public interface IImageService
{
    Task<Image> SaveImageAsync(ImageUploadResultDto uploadResult);
    Task<Image> GetImageAsync(ImageUploadResultDto uploadResult);
}

public class ImageService : IImageService
{
    private readonly ApplicationDbContext _context;

    public ImageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Image> SaveImageAsync(ImageUploadResultDto uploadResult)
    {
        var metadata = new
        {
            uploadResult.Width,
            uploadResult.Height,
            uploadResult.Format
        };

        var image = new Image
        {
            Id = Guid.NewGuid(),
            PublicId = uploadResult.PublicId,
            Url = uploadResult.Url,
            MetadatosJson = JsonSerializer.Serialize(metadata),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Images.Add(image);
        await _context.SaveChangesAsync();
        return image;
    }
    
    public async Task<Image> GetImageAsync(ImageUploadResultDto uploadResult)
    {
        var image = await _context.Images
            .FirstOrDefaultAsync(i => i.PublicId == uploadResult.PublicId);

        if (image == null)
        {
            image = await SaveImageAsync(uploadResult);
        }

        return image;
    }
}