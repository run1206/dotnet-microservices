using AutoMapper;
using PlatformService.Net7.Dtos;
using PlatformService.Net7.Models;

namespace PlatformService.Net7.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source --> Target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}