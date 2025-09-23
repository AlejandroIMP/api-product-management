namespace ProductManagement.Options;

public class CloudinaryOptions
{
    public string CloudName { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string ApiSecret { get; set; } = "";
    public string Folder { get; set; } = "";
    public long MaxFileSizeBytes { get; set; } = 4 * 1024 * 1024; 
}