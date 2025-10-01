using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProductManagement.Services.Abstract;
using ProductManagement.Services.Models;
using ProductManagement.Services.Options;

namespace ProductManagement.Services.ImageProcessing.Provider;

public class ImaggaImageProvider : IImageAiProvider
{
    private readonly ILogger<ImaggaImageProvider> _logger;
    private readonly HttpClient _http;
    private readonly string _endpoint;
    private readonly string? _key;
    private readonly string? _user;
    private readonly string? _secret;

    public string Name => "Imagga";

    public ImaggaImageProvider(HttpClient http, ILogger<ImaggaImageProvider> logger, IOptions<AiOptions>? options)
    {
        _http = http;
        _logger = logger;
        var opts = options?.Value ?? throw new InvalidOperationException("AI options are not configured.");
        _endpoint = opts.ImaggaEndpoint ?? throw new InvalidOperationException("AI: `ImaggaEndpoint` not configured.");
        _key = opts.ImaggaKey;
        _user = opts.ImaggaUser;
        _secret = opts.ImaggaSecret;
    }

    public async Task<ProviderResult> GenerateAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        // Build GET URL with image_url query parameter
        var uriBuilder = new UriBuilder(_endpoint);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["image_url"] = imageUrl;
        uriBuilder.Query = query.ToString() ?? string.Empty;
        var requestUri = uriBuilder.Uri;

        using var req = new HttpRequestMessage(HttpMethod.Get, requestUri);

        // Auth: prefer Basic (user:secret) if provided, otherwise Bearer with key if provided
        if (!string.IsNullOrEmpty(_user))
        {
            var secret = _secret ?? string.Empty;
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_user}:{secret}"));
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        }
        else if (!string.IsNullOrEmpty(_key))
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _key);
        }

        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage res;
        try
        {
            res = await _http.SendAsync(req, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Imagga request timed out for image {Url}", imageUrl);
            return new ProviderResult(false, null, IsTransient: true, ErrorMessage: "timeout");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP request to Imagga endpoint failed for image {Url}", imageUrl);
            return new ProviderResult(false, null, IsTransient: true, ErrorMessage: ex.Message);
        }

        var content = await res.Content.ReadAsStringAsync(cancellationToken);

        if (!res.IsSuccessStatusCode)
        {
            var isTransient = (int)res.StatusCode >= 500;
            _logger.LogWarning("Imagga returned {Status}. Body: {Body}", res.StatusCode, content);
            return new ProviderResult(false, null, IsTransient: isTransient, ErrorMessage: $"{res.StatusCode}");
        }

        try
        {
            var json = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return new ProviderResult(true, json, IsTransient: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed deserializing Imagga response. Raw body: {Body}", content);
            return new ProviderResult(false, null, IsTransient: false, ErrorMessage: "invalid_json");
        }
    }
}
