using ProductManagement.Services.Models;

namespace ProductManagement.Services.Abstract;

public interface IImageAiProvider
{
    string Name { get; }
    Task<ProviderResult> GenerateAsync(string imageUrl, CancellationToken cancellationToken = default);
}

