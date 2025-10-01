namespace ProductManagement.Services.Options;

public class ImaggaOptions
{
  public string Username { get; set; } = string.Empty; // acc_xxx
  public string ApiKey { get; set; } = string.Empty;   // secret part
  public int TimeoutSeconds { get; set; } = 60;
}