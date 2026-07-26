using AutoMapper;
using PlatformService.Net9.Dtos;
using PlatformService.Net9.Models;

namespace PlatformService.Net9.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source --> Target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}