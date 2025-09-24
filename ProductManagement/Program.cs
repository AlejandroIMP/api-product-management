using ProductManagement.Extensions;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Services;
using Microsoft.OpenApi.Models;
using ProductManagement.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Management API", Version = "v1" });
    c.OperationFilter<SwaggerFileOperationFilter>();
});

const string clientPolicy = "ClientPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(clientPolicy, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "https://localhost:7218",
                "https://127.0.0.1:7218")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
    
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

builder.Services.AddCloudinary(builder.Configuration);

builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(clientPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();