using Blog.Infrastructure.Images;
using Blog.Infrastructure.Minio;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;

namespace Blog.Services;

public interface IFileService
{
    Task<Dictionary<ImageVariant, string>> UploadPostImageAsync(IFormFile file, int postId, int imageIndex);
    Task<Dictionary<ImageVariant, string>> UploadAvatarAsync(IFormFile file, int userId);

    Task DeletePostImagesAsync(int postId);
    Task DeleteSinglePostImageAsync(string objectKey);
    Task DeleteAvatarAsync(int userId);
    
}

public class MinioFileService : IFileService
{
    public readonly ILogger<MinioObjectDeleter> _logger;
    private readonly IMinioClient _minio;
    private readonly IImageVariantGenerator _imageGenerator;

    public MinioFileService(IMinioClient minio, IImageVariantGenerator imageGenerator, ILogger<MinioObjectDeleter> logger)
    {
        _logger = logger;
        _minio = minio;
        _imageGenerator = imageGenerator;
    }

    public async Task<Dictionary<ImageVariant, string>> UploadPostImageAsync(IFormFile file, int postId, int imageIndex)
    {
        await using var stream = file.OpenReadStream();

        var variants = await _imageGenerator.GenerateAsync(stream, file.FileName);

        var result = new Dictionary<ImageVariant, string>();

        foreach (var variant in variants)
        {
            var objectName = $"posts/{postId}/{imageIndex}/{variant.FileNameSuffix}.jpg";

            using var ms = new MemoryStream(variant.Bytes);

            await _minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket("blog-images")
                .WithObject(objectName)
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType(variant.ContentType));

            result[variant.Variant] = $"/minio/blog-images/{objectName}";
        }

        return result;
    }

    public async Task<Dictionary<ImageVariant, string>> UploadAvatarAsync(IFormFile file, int userId)
    {
        await using var stream = file.OpenReadStream();

        var variants = await _imageGenerator.GenerateAvatarAsync(stream, file.FileName);

        var result = new Dictionary<ImageVariant, string>();

        foreach (var variant in variants)
        {
            var objectName = $"avatars/{userId}/{variant.FileNameSuffix}.jpg";

            using var ms = new MemoryStream(variant.Bytes);

            await _minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket("blog-avatars")
                .WithObject(objectName)
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType(variant.ContentType));

            result[variant.Variant] = $"/minio/blog-avatars/{objectName}";
        }

        return result;
    }


    public Task DeletePostImagesAsync(int postId)
        => DeleteByPrefixAsync("blog-images", $"posts/{postId}/");

    public Task DeleteAvatarAsync(int userId)
        => DeleteByPrefixAsync("blog-avatars", $"avatars/{userId}/");

    public Task DeleteSinglePostImageAsync(string objectKey)
        => DeleteByObjectKeyAsync("blog-images", objectKey);


    private async Task DeleteByObjectKeyAsync(string bucket, string objectKey)
    {
        var deleter = new MinioObjectDeleter(_minio, _logger);
        await deleter.DeleteAsync(bucket, objectKey);
    }

    private async Task DeleteByPrefixAsync(string bucket, string prefix)
    {
        var listArgs = new ListObjectsArgs()
            .WithBucket(bucket)
            .WithPrefix(prefix)
            .WithRecursive(true);

        var observable = _minio.ListObjectsAsync(listArgs);
        var deleter = new MinioObjectDeleter(_minio, _logger);

        await Parallel.ForEachAsync(
            observable.ToAsyncEnumerable(),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            },
            async (item, _) =>
            {
                await deleter.DeleteAsync(bucket, item.Key);
            });
    }


}
