using Minio;
using Minio.DataModel.Args;

namespace Blog.Infrastructure.Minio;

public sealed class MinioObjectDeleter
{
    private readonly IMinioClient _minio;
    private readonly ILogger<MinioObjectDeleter> _log;

    public MinioObjectDeleter(IMinioClient minio, ILogger<MinioObjectDeleter> log)
    {
        _minio = minio;
        _log = log;
    }

    public async Task DeleteAsync(string bucket, string key)
    {
        try
        {
            await _minio.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                );
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Ошибка удаления объекта {Bucket}/{Key}", bucket, key);
            throw;
        }
    }
}