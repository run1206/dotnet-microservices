using AutoMapper;
using PlatformService.Net6.Dtos;
using PlatformService.Net6.Models;

namespace PlatformService.Net6.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source --> Target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}