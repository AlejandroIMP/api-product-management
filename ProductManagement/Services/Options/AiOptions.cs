namespace ProductManagement.Services.Options;

public class AiOptions
{
  public string? Provider { get; set; } // e.g., "OpenAI", "Azure", etc.

  // Gemini specific
  public string? GeminiEndpoint { get; set; }
  public string? GeminiKey { get; set; }

  // Imagga specific
  public string? ImaggaEndpoint { get; set; }
  public string? ImaggaKey { get; set; }
  public string? ImaggaUser { get; set; }
  public string? ImaggaSecret { get; set; }

  // Backwards compatibility (previous single endpoint/key)
  public string? Endpoint { get; set; }
  public string? Key { get; set; }

  public string? Model { get; set; }        // optional: model/version identifier
  public int TimeoutSeconds { get; set; } = 30;
}