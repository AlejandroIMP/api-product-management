using Microsoft.AspNetCore.Mvc;
using ProductManagement.Services.Abstract;
using ProductManagement.Services.ImageProcessing;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageUpload _imageUpload;
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageUpload imageUpload, IImageService imageService, ILogger<ImageController> logger)
        {
            _imageUpload = imageUpload;
            _imageService = imageService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                _logger.LogInformation("Starting image upload...");

                if (image.Length == 0)
                {
                    _logger.LogWarning("No image file provided");
                    return BadRequest("Image file is required.");
                }

                _logger.LogInformation($"Uploading image: {image.FileName}, Size: {image.Length} bytes");

                var uploadResult = await _imageUpload.UploadImageAsync(image);
                _logger.LogInformation($"Image uploaded to Cloudinary: {uploadResult.PublicId}");

                var savedImage = await _imageService.SaveImageAsync(uploadResult);
                _logger.LogInformation($"Image saved to database with ID: {savedImage.Id}");

                return Ok(new
                {
                    ImageId = savedImage.Id.ToString(),
                    savedImage.PublicId,
                    savedImage.Url,
                    savedImage.MetadatosJson
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }
    }
}