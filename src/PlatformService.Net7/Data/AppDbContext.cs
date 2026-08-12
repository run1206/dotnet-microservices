using Microsoft.EntityFrameworkCore;
using PlatformService.Net7.Models;

namespace PlatformService.Net7.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Platform> Platforms { get; set; }
}