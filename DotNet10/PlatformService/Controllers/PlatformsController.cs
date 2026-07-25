using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController(IPlatformRepo repository, IMapper mapper) : ControllerBase
{
    private readonly IPlatformRepo _repository = repository;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platforms = _repository.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult GetPlatformById(int id)
    {
        var platform = _repository.GetPlatformById(id);
        if (platform == null)
            return NotFound();
        
        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public ActionResult<PlatformReadDto> CreatePlatflorm(PlatformCreateDto createDto)
    {
        var model = _mapper.Map<Platform>(createDto);

        _repository.CreatePlatform(model);
        _repository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(model);
        
        return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto );
    }
}