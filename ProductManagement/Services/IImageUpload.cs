namespace ProductManagement.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public interface IImageUpload
{
    Task<ImageUploadResultDto> UploadImageAsync(IFormFile file, string? filename = null, CancellationToken cancellationToken = default);
}

public record ImageUploadResultDto(string Url, string PublicId, int? Width, int? Height, string? Format);