using ProductManagement.Services.Models;

namespace ProductManagement.Services.Abstract;

public interface IImageAiService
{
  Task<ProviderResult> GenerateImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}
