using Microsoft.EntityFrameworkCore;
using PlatformService.Net9.Models;

namespace PlatformService.Net9.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Platform> Platforms { get; set; }
}