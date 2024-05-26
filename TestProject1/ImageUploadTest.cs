using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories.Implemantation;
using NZWalks.API.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    public class ImageUploadTest
    {
        
        private readonly Mock<IWebHostEnvironment> mockWebHostEnvironemnet;
        private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
        private readonly Mock<IFormFile> mockFormFile;
        private NZWalksDbContext dbContext;


        public ImageUploadTest()
        {
            mockWebHostEnvironemnet = new Mock<IWebHostEnvironment>();
            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockFormFile = new Mock<IFormFile>();
            var options = new DbContextOptionsBuilder<NZWalksDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            dbContext = new NZWalksDbContext(options);
            mockWebHostEnvironemnet.Setup(m => m.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost", 7091);
            mockHttpContextAccessor.Setup(ca=>ca.HttpContext).Returns(httpContext);
        }

        [Fact]
        public async Task Upload_ShouldUploadImageAndReturnImage()
        {
            //Arrange
            var fileName = "testImage";
            var fileExtension = ".jpg";
            var localFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", $"{fileName}{fileExtension}");
            // Ensure the directory exists
            var directoryPath = Path.GetDirectoryName(localFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var image = new Image
            {
                Id = Guid.NewGuid(),
                File = mockFormFile.Object,
                FileName = fileName,
                FileExtension = fileExtension,
                FileSizeinBytes = 1024 // Set a valid file size
            };
            using var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("This is a test file.");
            writer.Flush();
            ms.Position = 0;
            mockFormFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns((Stream target, CancellationToken token) => ms.CopyToAsync(target, token));

            var repository = new LocalImageRepository(mockWebHostEnvironemnet.Object, mockHttpContextAccessor.Object, dbContext);

            // Act
            var result = await repository.Upload(image);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"{fileName}{fileExtension}", result.FileName + result.FileExtension);
            Assert.Equal($"https://localhost:7091/Images/{fileName}{fileExtension}", result.FilePath);
            Assert.Equal(image.Id, result.Id);
            Assert.Equal(1, await dbContext.Images.CountAsync());

            // Clean up
            if (File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
            }
        }
    }
}
