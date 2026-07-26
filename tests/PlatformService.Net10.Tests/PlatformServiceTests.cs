using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PlatformService.Net10.Dtos;

namespace PlatformService.Net10.Tests;

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

        var response = await client.GetAsync("/api/platforms");

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

        var allPlatformsResponse = await client.GetAsync("/api/platforms");
        var allPlatforms = await allPlatformsResponse.Content.ReadFromJsonAsync<List<PlatformReadDto>>();

        var targetPlatform = Assert.Single(allPlatforms!, platform => platform.Name == "DotNet");

        var response = await client.GetAsync($"/api/platforms/{targetPlatform.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var platform = await response.Content.ReadFromJsonAsync<PlatformReadDto>();

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

        var response = await client.PostAsJsonAsync("/api/platforms", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdPlatform = await response.Content.ReadFromJsonAsync<PlatformReadDto>();

        Assert.NotNull(createdPlatform);
        Assert.Equal(createDto.Name, createdPlatform.Name);
        Assert.True(createdPlatform.Id > 0);
    }
}
