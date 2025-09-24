# Product Management API

A robust .NET 8 Web API for managing products with image upload capabilities using Cloudinary integration and SQL Server database.

## 🚀 Features

- **Product Management**: Complete CRUD operations for products
- **Image Upload**: Cloudinary integration for image storage and management
- **RESTful API**: Well-structured REST endpoints
- **Database Integration**: Entity Framework Core with SQL Server
- **Swagger Documentation**: Interactive API documentation
- **Docker Support**: Containerized deployment ready
- **CORS Support**: Configured for frontend integration

## 🛠️ Technologies Used

- **.NET 8**: Latest .NET framework
- **ASP.NET Core Web API**: Backend framework
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Database management system
- **Cloudinary**: Cloud-based image management
- **Swagger/OpenAPI**: API documentation
- **Docker**: Containerization

## 📋 Prerequisites

Before running this project, make sure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)
- Cloudinary account for image storage

## ⚙️ Installation & Setup

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd ProductManagement
```

### 2. Database Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=ProductManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCerticate=true"
  }
}
```

### 3. Cloudinary Configuration

Configure Cloudinary settings in `appsettings.json`:

```json
{
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### 4. Database Migration

Run the following commands to set up the database:

```bash
cd ProductManagement
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7218`
- HTTP: `http://localhost:5173`

## 🐳 Docker Deployment

### Build and Run with Docker

```bash
# Build the Docker image
docker build -t productmanagement .

# Run with Docker Compose
docker-compose up
```

## 📚 API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/product` | Get all products |
| GET | `/api/product/{id}` | Get product by ID |
| POST | `/api/product` | Create new product |
| PUT | `/api/product/{id}` | Update existing product |
| DELETE | `/api/product/{id}` | Delete product |

### Images

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/image/upload` | Upload image to Cloudinary |
| DELETE | `/api/image/{publicId}` | Delete image from Cloudinary |

## 📖 API Documentation

Once the application is running, you can access the interactive Swagger documentation at:
- `https://localhost:7218/swagger` (in development mode)

## 🏗️ Project Structure

```
ProductManagement/
├── Controllers/           # API Controllers
│   ├── ProductController.cs
│   └── ImageController.cs
├── Data/                 # Database Context
│   └── ApplicationDbContext.cs
├── Models/               # Data Models and DTOs
│   ├── Entities/
│   │   ├── Product.cs
│   │   └── Image.cs
│   ├── AddProductDto.cs
│   ├── UpdateProductDto.cs
│   ├── ProductResponseDto.cs
│   └── ImageDto.cs
├── Services/             # Business Logic Services
│   ├── IProductService.cs
│   ├── ProductService.cs
│   ├── IImageService.cs
│   └── ImageService.cs
├── Extensions/           # Service Extensions
│   └── CloudinaryServiceCollectionExtensions.cs
├── Filters/              # Custom Filters
│   └── SwaggerFileOperationFilter.cs
├── Middleware/           # Custom Middleware
│   └── ImageUploadMiddleware.cs
├── Migrations/           # EF Core Migrations
├── Options/              # Configuration Options
│   └── CloudinaryOptions.cs
└── Properties/           # Launch Settings
    └── launchSettings.json
```

## 🔧 Configuration

### CORS Settings

The API is configured to accept requests from:
- `http://localhost:5173`
- `http://127.0.0.1:5173`
- `https://localhost:7218`
- `https://127.0.0.1:7218`

### Database Models

#### Product Entity
- `Id`: Unique identifier (Guid)
- `Name`: Product name (max 100 characters)
- `Description`: Product description (max 255 characters)
- `Price`: Product price (decimal, must be > 0)
- Additional properties for timestamps and relationships


---

