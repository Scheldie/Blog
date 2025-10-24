using Blog.Data;
using Blog.Entities;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Blog.Services
{
    public class ImageService
    {
        private readonly IMinioClient minio;
        private readonly ILogger logger;
        private readonly IConfiguration config;
        private readonly BlogDbContext repository;

        public ImageService(IMinioClient minio, ILogger logger, IConfiguration config)
        {
            this.minio = minio;
            this.logger = logger;
            this.config = config;
        }
        public string GenerateKey() => $"{Guid.NewGuid()}";

        public async Task<string> UploadAvatar(IFormFile file)
        {
            var bucket = config["Minio:bucket-avatars"];
            return UploadImage(file, bucket).Result;
        }
        public async Task<string> UploadPostImage(IFormFile file)
        {
            var bucket = config["Minio:bucket-posts"];
            return UploadImage(file, bucket).Result;
        }
        private async Task<string> UploadImage(IFormFile file, string bucket)
        {
            var key = GenerateKey();

            if (file == null || file.Length == 0)
            {
                throw new Exception("Image is not selected");
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
                return $"{key}.jpg";

            }
            catch (MinioException e)
            {
                throw new Exception($"Uploading error: {e.Message}");
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
                throw new Exception("No paths to image");
            }
            try
            {
                var statArgs = new StatObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(image.Path);

                var metadata = await minio.StatObjectAsync(statArgs);

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
