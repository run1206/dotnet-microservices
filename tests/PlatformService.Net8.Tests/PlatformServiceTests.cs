using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PlatformService.Net8.Data;
using PlatformService.Net8.Dtos;
using PlatformService.Net8.Models;

namespace PlatformService.Net8.Tests;

public class PlatformServiceTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task GetAllPlatforms_ReturnsSeededPlatforms()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/platforms", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var platforms = await response.Content.ReadFromJsonAsync<List<PlatformReadDto>>(TestContext.Current.CancellationToken);

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

        var allPlatformsResponse = await client.GetAsync("/api/platforms", TestContext.Current.CancellationToken);
        var allPlatforms = await allPlatformsResponse.Content.ReadFromJsonAsync<List<PlatformReadDto>>(TestContext.Current.CancellationToken);

        var targetPlatform = Assert.Single(allPlatforms!, platform => platform.Name == "DotNet");

        var response = await client.GetAsync($"/api/platforms/{targetPlatform.Id}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var platform = await response.Content.ReadFromJsonAsync<PlatformReadDto>(TestContext.Current.CancellationToken);

        Assert.NotNull(platform);
        Assert.Equal(targetPlatform.Id, platform.Id);
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

        var response = await client.PostAsJsonAsync("/api/platforms", createDto, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdPlatform = await response.Content.ReadFromJsonAsync<PlatformReadDto>(TestContext.Current.CancellationToken);

        Assert.NotNull(createdPlatform);
        Assert.Equal(createDto.Name, createdPlatform.Name);
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

            seedDataMethod!.Invoke(null, [context]);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Equal(1, context.Platforms.Count());
        Assert.Contains("We already have data", output.ToString());
    }
}