using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Net10.Data;
using PlatformService.Net10.Dtos;
using PlatformService.Net10.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 1. THIS REPLACES Startup.ConfigureServices

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemory"));
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();


builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
// 2. THIS REPLACES Startup.Configure
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/api/platforms",
    (IPlatformRepo repository, IMapper mapper) =>
    {
        var platforms = repository.GetAllPlatforms();
        return Results.Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    });

app.MapGet("/api/platforms/{id}",
    (int id, IPlatformRepo repository, IMapper mapper) =>
    {
        var platform = repository.GetPlatformById(id);
        return platform is null
            ? Results.NotFound()
            : Results.Ok(mapper.Map<PlatformReadDto>(platform));
    })
    .WithName("GetPlatformById");

app.MapPost("/api/platforms",
    (PlatformCreateDto createDto, IPlatformRepo repository, IMapper mapper) =>
    {
        var model = mapper.Map<Platform>(createDto);
        repository.CreatePlatform(model);
        repository.SaveChanges();

        var platformReadDto = mapper.Map<PlatformReadDto>(model);
        return Results.CreatedAtRoute("GetPlatformById",
            new { id = platformReadDto.Id }, platformReadDto);
    });

PrepDb.PrepPopulation(app);

app.Run();

public partial class Program { }