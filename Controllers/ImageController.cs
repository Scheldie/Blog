using Blog.Data;
using Blog.Entities;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System.Security.AccessControl;
using System.Security.Claims;

namespace Blog.Controllers
{
    public class ImageController : Controller
    {
        private readonly IMinioClient minio;
        private readonly ILogger logger;
        private readonly IConfiguration config;
        private readonly BlogDbContext repository;
        
        public ImageController(IMinioClient minio, ILogger logger, IConfiguration config) 
        {
            this.minio = minio;
            this.logger = logger;
            this.config = config;
        }
        public string GenerateKey() => $"{Guid.NewGuid()}";
        
        [HttpPut]
        public async void UploadAvatar(IFormFile file)
        {
            var bucket = config["Minio:bucket-avatars"];
            UploadImage(file, bucket);
        }
        [HttpPut]
        public async void UploadPostImage(IFormFile file)
        {
            var bucket = config["Minio:bucket-posts"];
            UploadImage(file, bucket);
        }
        private async void UploadImage(IFormFile file, string bucket)
        {
            var key = GenerateKey();

            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Image is not selected";
                throw new ArgumentNullException(nameof(file));
            }
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucket);
                if (!await minio.BucketExistsAsync(bucketExistsArgs))
                {
                    await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket));
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject($"{key}.jpg")
                    .WithStreamData(file.OpenReadStream())
                    .WithObjectSize(file.Length)
                    .WithContentType("image/webp");

                await minio.PutObjectAsync(putObjectArgs);
                
            }
            catch (MinioException e)
            {
                ViewBag.Message = $"Uploading error: {e.Message}";
            }
        }
        public async Task<byte[]> LoadAvatar(Image image)
        {
            var bucket = config["Minio:bucket-avatars"];
            return LoadImage(image, bucket).Result;
        }
        public async Task<byte[]> LoadPostImage(Image image)
        {
            var bucket = config["Minio:bucket-posts"];
            return LoadImage(image, bucket).Result;
        }
        [HttpGet]
        public async Task<byte[]> LoadImage(Image image, string bucket)
        {
            var key = image.Path;

            if (key == null)
            {
                ViewBag.Message = "No paths to image";
                return null;
            }
            try
            {
                var statArgs = new StatObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(image.Path);

                var metadata = await minio.StatObjectAsync(statArgs);

                // Затем получаем данные
                var memoryStream = new MemoryStream();
                var getArgs = new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(image.Path)
                    .WithCallbackStream(async (stream, token) =>
                    {
                        await stream.CopyToAsync(memoryStream);
                    });

                await minio.GetObjectAsync(getArgs);
                var contentType = GetContentType(key);

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Image not found: {ex.Message}");
            }
        }
        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

    }
}
