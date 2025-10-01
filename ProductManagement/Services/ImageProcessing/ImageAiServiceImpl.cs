using ProductManagement.Services.Abstract;
using ProductManagement.Services.Models;

namespace ProductManagement.Services.ImageProcessing;

public class ImageAiServiceImpl : IImageAiService
{
    private readonly IEnumerable<IImageAiProvider> _providers;
    private readonly ILogger<ImageAiServiceImpl> _logger;

    public ImageAiServiceImpl(IEnumerable<IImageAiProvider> providers, ILogger<ImageAiServiceImpl> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task<ProviderResult> GenerateImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        foreach (var p in _providers)
        {
            _logger.LogInformation("Calling AI provider {Provider} for {Url}", p.Name, imageUrl);
            ProviderResult res;
            try
            {
                res = await p.GenerateAsync(imageUrl, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provider {Provider} threw an exception", p.Name);
                // treat exception as transient and try next provider
                continue;
            }

            if (res.Success)
            {
                return res;
            }

            _logger.LogWarning("Provider {Provider} failed. Success={Success} Transient={Transient} Error={Error}", p.Name, res.Success, res.IsTransient, res.ErrorMessage);

            if (!res.IsTransient)
            {
                // permanent failure - stop and return the provider result
                return res;
            }

            // otherwise transient - try next provider
        }

        // all providers failed (transient)
        return new ProviderResult(false, null, IsTransient: true, ErrorMessage: "all_providers_failed");
    }
}
