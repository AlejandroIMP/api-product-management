using Microsoft.EntityFrameworkCore;
using ProductManagement.Models.Entities;

namespace ProductManagement.Data;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Image> Images { get; set; }
}