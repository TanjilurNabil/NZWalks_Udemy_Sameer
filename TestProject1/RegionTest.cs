
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NZWalks.API.Controllers;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories.Interfaces;

namespace TestProject1
{
    public class RegionTest
    {
        //RegionsController regionController = new RegionsController(null,null);
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IRegionRepository> _mockRepo;
        public RegionTest()
        {
            _mapper = new Mock<IMapper>();
            _mockRepo = new Mock<IRegionRepository>();
        }
        [Fact]
        public async Task GetAllRegion_Should_ReturnAllRegion()
        {
            //Arrange
            
            var regionsDomain = new List<Region>
            {
                new Region {Id = Guid.NewGuid(), Name = "Region 1", Code="R1",RegionImageUrl = " " },
                new Region {Id = Guid.NewGuid(), Name = "Region 2", Code="R2",RegionImageUrl = " " },
            };
            var regionsDto = new List<RegionDto>
            {
                new RegionDto {Id = regionsDomain[0].Id, Name = "Region 1", Code="R1",RegionImageUrl = " " },
                new RegionDto {Id = regionsDomain[1].Id, Name = "Region 2", Code="R2",RegionImageUrl = " " },
            };

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(regionsDomain);
            _mapper.Setup(mapper=>mapper.Map<List<RegionDto>>(regionsDomain)).Returns(regionsDto);
           var regionController = new RegionsController(_mockRepo.Object,_mapper.Object);
            //Act
            var result = await regionController.GetAll();

            //Assert
            //Fluent


            //result.Should().BeOfType<List<RegionDto>>();
            //result.Should().NotBeNull();



            //XUnit default Assertion
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returndRegionDto = Assert.IsType<List<RegionDto>>(okResult.Value);
            Assert.Equal(regionsDto.Count, returndRegionDto.Count);

        }
        [Fact]
        public async Task Should_Create_Region_Successfully()
        {
            //Arrange
            var regionDomain = new Region();
            var addRegionDto = new AddRegionRequestDto
            {
                
                Name = "Region 1",
                Code = "R1",
                RegionImageUrl = "sampleImage.jpg"
            };
            var createdRegion = new Region
            {
                Id = Guid.NewGuid(),
                Name = "Region 1",
                Code = "R1",
                RegionImageUrl = "sampleImage.jpg"
            };

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Region>())).ReturnsAsync(createdRegion);
            _mapper.Setup(mapper => mapper.Map<Region>(addRegionDto)).Returns(regionDomain);
            _mapper.Setup(mapper => mapper.Map<RegionDto>(createdRegion)).Returns(new RegionDto
            {
                Id = createdRegion.Id,
                Name = createdRegion.Name,
                Code = createdRegion.Code,
                RegionImageUrl = createdRegion.RegionImageUrl
            });
            var regionController= new RegionsController(_mockRepo.Object, _mapper.Object);
            //Act
            var result = await regionController.Create(addRegionDto);
            //Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedRegionDto = Assert.IsType<RegionDto>(createdAtActionResult.Value);

            Assert.Equal(createdRegion.Id, createdAtActionResult.RouteValues["id"]);
        }
    }
}