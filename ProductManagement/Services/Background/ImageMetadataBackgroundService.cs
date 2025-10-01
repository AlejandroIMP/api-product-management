using System.Text.Json;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Services.Abstract;

namespace ProductManagement.Services.Background;

public class ImageMetadataBackgroundService : BackgroundService
{
  private readonly ILogger<ImageMetadataBackgroundService> _logger;
  private readonly IServiceProvider _serviceProvider;
  private readonly IBackgroundTaskQueue _taskQueue;

  public ImageMetadataBackgroundService(ILogger<ImageMetadataBackgroundService> logger,
    IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue)
  {
    _taskQueue = taskQueue;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }
  
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var imageId = await _taskQueue.DequeueAsync(stoppingToken);
            if (imageId == null) break;

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var ai = scope.ServiceProvider.GetRequiredService<IImageAiService>();

                var image = await db.Images.FindAsync(new object[] { imageId.Value }, stoppingToken);
                if (image == null) continue;

                // Marca como Processing
                image.MetadataStatus = ImageEnum.Processing.ToString();
                image.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);

                // Llamar al servicio de IA
                var result = await ai.GenerateImageAsync(image.Url, stoppingToken);

                if (result.Success && result.Result.HasValue)
                {
                    // Guardar metadatos serializados
                    // JsonElement -> string
                    image.MetadatosJson = JsonSerializer.Serialize(result.Result.Value, new JsonSerializerOptions { WriteIndented = false });
                    image.MetadataStatus = ImageEnum.Completed.ToString();
                    image.UpdatedAt = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                }
                else
                {
                    _logger.LogWarning("AI providers failed for image {ImageId}. Error: {Error} Transient: {Transient}", image.Id, result.ErrorMessage, result.IsTransient);
                    image.MetadataStatus = ImageEnum.Failed.ToString();
                    image.UpdatedAt = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image metadata {ImageId}", imageId);
                // Intentar marcar como Failed
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var image = await db.Images.FindAsync(new object[] { imageId.Value }, CancellationToken.None);
                    if (image != null)
                    {
                        image.MetadataStatus = ImageEnum.Failed.ToString();
                        image.UpdatedAt = DateTime.UtcNow;
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    /* swallow */
                       
                }
            }
        }
    }
}