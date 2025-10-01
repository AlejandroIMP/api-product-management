// csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Services.Abstract;
using ProductManagement.Services.ImageProcessing;
using ProductManagement.Services.ImageProcessing.Provider;
using ProductManagement.Services.Options;
using ProductManagement.Services.Background;

namespace ProductManagement.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Options
            services.Configure<AiOptions>(config.GetSection("AI"));
            services.Configure<ImaggaOptions>(config.GetSection("Imagga"));
            services.Configure<GeminiOptions>(config.GetSection("Gemini"));

            // Http clients for providers
            services.AddHttpClient<GeminiImageProvider>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(config.GetValue<int?>("AI:TimeoutSeconds") ?? 60);
            });
            // Register provider implementations in fallback order (Gemini first)
            services.AddTransient<IImageAiProvider, GeminiImageProvider>();

            services.AddHttpClient<ImaggaImageProvider>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(config.GetValue<int?>("AI:TimeoutSeconds") ?? 60);
            });
            services.AddTransient<IImageAiProvider, ImaggaImageProvider>();

            // Orchestrator that takes IEnumerable<IImageAiProvider> and tries them in order
            services.AddScoped<IImageAiService, ImageAiServiceImpl>();

            // Other services / hosted services / background queue
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<ImageMetadataBackgroundService>();

            // Cloudinary, product, image services etc (assume existing extension)
            services.AddCloudinary(config);
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
