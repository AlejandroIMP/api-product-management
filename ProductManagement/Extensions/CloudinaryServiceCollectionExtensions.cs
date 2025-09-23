using CloudinaryDotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ProductManagement.Options;
using ProductManagement.Services;

namespace ProductManagement.Extensions;

public static class CloudinaryServiceCollectionExtensions
{
    public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CloudinaryOptions>(configuration.GetSection("CloudinarySettings"));
        var options = configuration.GetSection("CloudinarySettings").Get<CloudinaryOptions>()
            ?? throw new InvalidOperationException("Cloudinary configuration is missing.");
        
        var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
        services.AddSingleton(new Cloudinary(account));
        services.AddSingleton<IImageUpload, CloudinaryImageStorage>();
        return services;
    }
}