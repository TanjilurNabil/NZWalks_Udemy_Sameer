using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories.Interfaces;

namespace NZWalks.API.Repositories.Implemantation
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContext;
        private readonly NZWalksDbContext dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,IHttpContextAccessor httpContext,NZWalksDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContext = httpContext;
            this.dbContext = dbContext;
        }
        public async Task<Image> Upload(Image image)
        {
            //wehHostEnv navigate to the api root to explore folder
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath,"Images",
                $"{image.FileName}{image.FileExtension}");
            //Upload Image to local path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            //https://localhost:7189/images/image.jpg
            // scheme = http or https Host = localhost or the domainName 
            var urlFilePath = $"{httpContext.HttpContext.Request.Scheme}://{httpContext.HttpContext.Request.Host}{httpContext.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            // asign uploaded image url to domain for database reference
            image.FilePath = urlFilePath;

            //Add Image to database

            await dbContext.Images.AddAsync(image);
            await dbContext.SaveChangesAsync();

            return image;
        }
    }
}
