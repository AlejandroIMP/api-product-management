using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using ProductManagement.Services.Abstract;
using ProductManagement.Services.Options;

namespace ProductManagement.Services.Storage;

public class CloudinaryImageStorage : IImageUpload
{
    private readonly Cloudinary _cloudinary;
    private readonly long _maxFileSizeBytes;
    private readonly string _folder;

    public CloudinaryImageStorage(Cloudinary cloudinary, IOptions<CloudinaryOptions> options)
    {
        _cloudinary = cloudinary;
        _folder = options.Value.Folder;
        _maxFileSizeBytes = options.Value.MaxFileSizeBytes;
    }
    
    public async Task<ImageUploadResultDto> UploadImageAsync(IFormFile file, string? filename = null, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0) throw new ArgumentException("File size cannot be zero or negative.", nameof(file));
        if (file.Length > _maxFileSizeBytes)
        {
            throw new InvalidOperationException($"File size exceeds the maximum limit of {_maxFileSizeBytes} bytes.");
        }

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(filename ?? file.FileName, stream),
            Folder = _folder,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception("Image upload failed: " + uploadResult.Error?.Message);
        }

        return new ImageUploadResultDto(
            Url: uploadResult.SecureUrl.ToString(),
            PublicId: uploadResult.PublicId,
            Width: uploadResult.Width,
            Height: uploadResult.Height,
            Format: uploadResult.Format
        );
    }
}