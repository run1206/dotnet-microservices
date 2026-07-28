using AutoMapper;
using PlatformService.Net8.Dtos;
using PlatformService.Net8.Models;

namespace PlatformService.Net8.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source --> Target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}