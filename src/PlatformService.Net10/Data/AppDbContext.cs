using Microsoft.EntityFrameworkCore;
using PlatformService.Net10.Models;

namespace PlatformService.Net10.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Platform> Platforms { get; set; }
}