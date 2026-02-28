namespace Blog.Infrastructure.Images;

public enum ImageVariant
{
    Thumbnail,
    Preview,
    Full,
    Original,
    
    AvatarSmall32, 
    AvatarSmall40, 
    AvatarProfile, 
    AvatarFull
}

public sealed class ImageVariantResult
{
    public required ImageVariant Variant { get; init; }
    public required string FileNameSuffix { get; init; } // "thumb", "preview", ...
    public required string ContentType { get; init; }    // "image/jpeg"
    public required byte[] Bytes { get; init; }
}

public interface IImageVariantGenerator
{
    Task<IReadOnlyList<ImageVariantResult>> GenerateAsync(
        Stream input,
        string originalFileName,
        CancellationToken ct = default);

    Task<IReadOnlyList<ImageVariantResult>> GenerateAvatarAsync(
        Stream input,
        string originalFileName,
        CancellationToken ct = default);
}