using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using Microsoft.OpenApi.Models;
using ProductManagement.Filters;
using ProductManagement.Services.Extensions;

// Crea el builder para configurar la aplicación web
var builder = WebApplication.CreateBuilder(args);

// Configura los servicios de la aplicación

// Habilita el uso de controladores (API Controllers)
builder.Services.AddControllers();
// Agrega la exploración de endpoints para Swagger
builder.Services.AddEndpointsApiExplorer();
// Configura Swagger para la documentación de la API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Management API", Version = "v1" });
    c.OperationFilter<SwaggerFileOperationFilter>(); // Filtro personalizado para subida de archivos
});

// Define el nombre de la política CORS
const string clientPolicy = "ClientPolicy";
// Configura CORS para permitir peticiones desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(clientPolicy, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",    // Vite dev server (HTTP)
                "http://127.0.0.1:5173",   // Vite dev server alternativo
                "https://localhost:7218",  // HTTPS local
                "https://127.0.0.1:7218")  // HTTPS alternativo
            .AllowAnyHeader()      // Permite cualquier header HTTP
            .AllowAnyMethod()      // Permite todos los métodos (GET, POST, PUT, DELETE)
            .AllowCredentials();   // Permite el envío de cookies/credenciales
    });
});
    
// Configura la conexión a la base de datos SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

// Registra servicios personalizados y opciones desde la configuración
builder.Services.AddApplicationServices(builder.Configuration);

// Construye la aplicación con todas las configuraciones
var app = builder.Build();

// Configura el pipeline de peticiones HTTP

// Solo en desarrollo: habilita Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilita Swagger en todos los ambientes (desarrollo y producción)
app.UseSwagger();
app.UseSwaggerUI();

// Aplica la política CORS configurada anteriormente
app.UseCors(clientPolicy);

// Fuerza el uso de HTTPS
app.UseHttpsRedirection();

// Habilita la autorización 
app.UseAuthorization();

// Mapea los controladores para que respondan a las rutas HTTP
app.MapControllers();

// Inicia la aplicación web
app.Run();