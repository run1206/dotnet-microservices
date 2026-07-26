using AutoMapper;
using PlatformService.Net10.Dtos;
using PlatformService.Net10.Models;

namespace PlatformService.Net10.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source --> Target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}