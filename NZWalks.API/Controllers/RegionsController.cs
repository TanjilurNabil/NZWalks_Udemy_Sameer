using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories.Interfaces;
using static System.Net.WebRequestMethods;

namespace NZWalks.API.Controllers
{
    //https://localhost:port/api/regions
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegionsController : ControllerBase
    {

        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {

            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }
        //Get all region
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Get Data from database - Domain Models
            var regionsDomain = await regionRepository.GetAllAsync();

            //Map Domain to DTO's

            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);
            //Return DTO's
            return Ok(regionsDto);
        }
        //Get Region by Id
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id); 
            var region = await regionRepository.GetByIdAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            var regionDto = mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }
        //POST To create New Region
        [HttpPost]
        [ValidateModel]
        
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            
                //Map DTo to Domain
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);
                //Create Region
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);
                //Map Domain back to DTO
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionDto);
            
            
        }
        //Update region
        //PUT https://localhost:port/api/region/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegion)
        {
            
                //Map DTo to Domain Model
                var regionDomainModel = mapper.Map<Region>(updateRegion);
                //Check If region exists
                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                //Convert Domain to DTO
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                return Ok(regionDto);
           
            

        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            return Ok(regionDto);
        }
    }
}
