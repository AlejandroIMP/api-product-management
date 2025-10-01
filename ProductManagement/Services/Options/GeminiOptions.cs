namespace ProductManagement.Services.Options;

public class GeminiOptions
{
  public string Endpoint { get; set; } = "";
  public string Key { get; set; } = "";
  public string Model { get; set; } = "gemini-1.5-pro"; // default model
  public int TimeoutSeconds { get; set; } = 30;
}