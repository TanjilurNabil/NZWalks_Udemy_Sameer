using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories.Interfaces;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper,IWalkRepository walkRepository) {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        //Create Walk
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            //Map Inputed DTo to Domain model
            var walkDomainModel =  mapper.Map<Walk>(addWalkRequestDto);
            await walkRepository.CreateAsync(walkDomainModel);
            //Map Domain Model to Dto
            var walkDto = mapper.Map<WalkDto>(walkDomainModel);
            return Ok(walkDto);

        }
    }
}
