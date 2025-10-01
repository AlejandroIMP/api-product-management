namespace ProductManagement.Services.Abstract;

public interface IImageUpload
{
    Task<ImageUploadResultDto> UploadImageAsync(IFormFile file, string? filename = null, CancellationToken cancellationToken = default);
}

public record ImageUploadResultDto(string Url, string PublicId, int? Width, int? Height, string? Format);