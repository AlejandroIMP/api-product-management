using Microsoft.AspNetCore.Mvc;
using ProductManagement.Services;

namespace ProductManagement.Middleware;

public class ImageUploadMiddleware : Controller
{
    private readonly RequestDelegate _next;
    public ImageUploadMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IImageUpload storage)
    {
        if(context.Request.Path.StartsWithSegments("/api/Product")
           && context.Request.Path.Value?.EndsWith("image") == true 
           && HttpMethods.IsPost(context.Request.Method))
        {
            if (!context.Request.HasFormContentType)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Please provide an image upload file.");
            }
            var form = await context.Request.ReadFormAsync();
            var file = form.Files.GetFile("image");
            if (file == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("file field is required.");
            }

            try
            {
                var result = await storage.UploadImageAsync(file!);
                var savedImage = await context.RequestServices.GetRequiredService<IImageService>()
                    .GetImageAsync(result);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsJsonAsync(new
                {
                    savedImage.Id,
                    savedImage.PublicId,
                    savedImage.Url,
                    savedImage.MetadatosJson
                });
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex.Message);
            }
        }
        await _next(context);
    }
}