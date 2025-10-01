using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.Entities;
using ProductManagement.Services.Abstract;

namespace ProductManagement.Services.ImageProcessing;

// Interfaz que define los métodos para manejar imágenes
public interface IImageService
{
    // Guarda una nueva imagen en la base de datos
    Task<Image> SaveImageAsync(ImageUploadResultDto uploadResult);
    // Obtiene una imagen existente o la crea si no existe
    Task<Image> GetImageAsync(ImageUploadResultDto uploadResult);
}

// Servicio que maneja la lógica de negocio para las imágenes
public class ImageService : IImageService
{
    // Contexto de la base de datos para acceder a las tablas
    private readonly ApplicationDbContext _context;
    private readonly IBackgroundTaskQueue _metadataQueue;

    // Constructor que recibe el contexto de base de datos
    public ImageService(ApplicationDbContext context, IBackgroundTaskQueue metadataQueue)
    {
        _context = context;
        _metadataQueue = metadataQueue;
    }

    // Método que guarda una nueva imagen en la base de datos
    public async Task<Image> SaveImageAsync(ImageUploadResultDto uploadResult)
    {
        // Crea un objeto con los metadatos de la imagen (ancho, alto, formato)
        var metadata = new
        {
            uploadResult.Width,
            uploadResult.Height,
            uploadResult.Format
        };

        // Crea una nueva entidad Image con todos los datos necesarios
        var image = new Image
        {
            Id = Guid.NewGuid(), // Genera un ID único
            PublicId = uploadResult.PublicId, // ID público de Cloudinary
            Url = uploadResult.Url, // URL donde está almacenada la imagen
            MetadatosJson = JsonSerializer.Serialize(metadata), // Metadatos en formato JSON
            MetadataStatus = ImageEnum.Pending.ToString(), // Estado inicial de los metadatos
            CreatedAt = DateTime.UtcNow, // Fecha de creación
            UpdatedAt = DateTime.UtcNow // Fecha de última actualización
        };
        
        // Agrega la imagen al contexto y la guarda en la base de datos
        _context.Images.Add(image);
        await _context.SaveChangesAsync();
        await _metadataQueue.QueueBackgroundWorkItemAsync(image.Id);
        return image;
    }
    
    // Método que busca una imagen existente o la crea si no existe
    public async Task<Image> GetImageAsync(ImageUploadResultDto uploadResult)
    {
        // Busca la imagen en la base de datos usando el PublicId
        var image = await _context.Images
            .FirstOrDefaultAsync(i => i.PublicId == uploadResult.PublicId);

        // Si no existe la imagen, la crea y guarda
        if (image == null)
        {
            image = await SaveImageAsync(uploadResult);
        }

        // Retorna la imagen encontrada o recién creada
        return image;
    }
}