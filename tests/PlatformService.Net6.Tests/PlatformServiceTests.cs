using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PlatformService.Net6.Data;
using PlatformService.Net6.Dtos;
using PlatformService.Net6.Models;

namespace PlatformService.Net6.Tests;

public class PlatformServiceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PlatformServiceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetAllPlatforms_ReturnsSeededPlatforms()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/platforms", CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var platforms = await response.Content.ReadFromJsonAsync<List<PlatformReadDto>>();

        Assert.NotNull(platforms);
        Assert.NotEmpty(platforms);
        Assert.Contains(platforms, platform => platform.Name == "DotNet");
    }

    [Fact]
    public async Task GetPlatformById_ReturnsPlatformWhenItExists()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var allPlatformsResponse = await client.GetAsync("/api/platforms", CancellationToken.None);
        var allPlatforms = await allPlatformsResponse.Content.ReadFromJsonAsync<List<PlatformReadDto>>();

        var targetPlatform = Assert.Single(allPlatforms!, platform => platform.Name == "DotNet");

        var response = await client.GetAsync($"/api/platforms/{targetPlatform.Id}", CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var platform = await response.Content.ReadFromJsonAsync<PlatformReadDto>();

        Assert.NotNull(platform);
        Assert.Equal(targetPlatform.Id, platform!.Id);
        Assert.Equal(targetPlatform.Name, platform.Name);
    }

    [Fact]
    public async Task PostPlatform_CreatesPlatformAndReturnsCreated()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var createDto = new PlatformCreateDto
        {
            Name = $"TestPlatform-{Guid.NewGuid():N}",
            Publisher = "Test Publisher",
            Cost = "Free"
        };

        var response = await client.PostAsJsonAsync("/api/platforms", createDto, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdPlatform = await response.Content.ReadFromJsonAsync<PlatformReadDto>();

        Assert.NotNull(createdPlatform);
        Assert.Equal(createDto.Name, createdPlatform!.Name);
        Assert.True(createdPlatform.Id > 0);
    }

    [Fact]
    public void SeedData_WhenPlatformsAlreadyExist_LogsMessageAndDoesNotSeedNewData()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        context.Platforms.Add(new Platform { Name = "Existing", Publisher = "Test", Cost = "Free" });
        context.SaveChanges();

        var output = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(output);

        try
        {
            var seedDataMethod = typeof(PrepDb).GetMethod("SeedData", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(seedDataMethod);

            seedDataMethod!.Invoke(null, new object?[] { context });
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Equal(1, context.Platforms.Count());
        Assert.Contains("We already have data", output.ToString());
    }
}