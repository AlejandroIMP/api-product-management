// csharp
// File: `ProductManagement/Services/ImageAiService.cs`

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProductManagement.Services.Abstract;
using ProductManagement.Services.Models;
using ProductManagement.Services.Options;

namespace ProductManagement.Services.ImageProcessing.Provider;
public class GeminiImageProvider : IImageAiProvider
{
    private readonly ILogger<GeminiImageProvider> _logger;
    private readonly HttpClient _http;
    private readonly string _aiEndpoint;
    private readonly string _aiKey;

    public string Name => "Gemini";

    public GeminiImageProvider(HttpClient http, ILogger<GeminiImageProvider> logger, IOptions<AiOptions>? options)
    {
        _http = http;
        _logger = logger;

        var opts = options?.Value ?? throw new InvalidOperationException("AI options are not configured.");
        _aiEndpoint = opts.GeminiEndpoint ?? opts.Endpoint ?? throw new InvalidOperationException("AI: `GeminiEndpoint` not configured.");
        _aiKey = opts.GeminiKey ?? opts.Key ?? throw new InvalidOperationException("AI: `GeminiKey` not configured.");
    }

    public async Task<ProviderResult> GenerateAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var prompt = $"Analyze image at {imageUrl}. Return caption, labels, tags, colors, faces, objects, alt_text, moderation_tags, dominant_color_hex, scene_type, text_in_image emotions and EXIF info in JSON.";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, _aiEndpoint);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        req.Headers.Add("X-goog-api-key", _aiKey);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage res;
        try
        {
            res = await _http.SendAsync(req, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // timeout
            _logger.LogWarning(ex, "Gemini request timed out for image {Url}", imageUrl);
            return new ProviderResult(false, null, IsTransient: true, ErrorMessage: "timeout");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP request to Gemini endpoint failed for image {Url}", imageUrl);
            return new ProviderResult(false, null, IsTransient: true, ErrorMessage: ex.Message);
        }

        var content = await res.Content.ReadAsStringAsync(cancellationToken);

        if (!res.IsSuccessStatusCode)
        {
            // Treat 5xx as transient so fallback can try another provider
            var isTransient = (int)res.StatusCode >= 500;
            _logger.LogWarning("Gemini returned {Status}. Body: {Body}", res.StatusCode, content);
            return new ProviderResult(false, null, IsTransient: isTransient, ErrorMessage: $"{res.StatusCode}");
        }

        try
        {
            var json = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return new ProviderResult(true, json, IsTransient: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed deserializing Gemini response. Raw body: {Body}", content);
            return new ProviderResult(false, null, IsTransient: false, ErrorMessage: "invalid_json");
        }
    }
}