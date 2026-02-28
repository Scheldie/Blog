using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Blog.Infrastructure.Images;

public sealed class ImageSharpImageVariantGenerator : IImageVariantGenerator
{
    private const int ThumbnailMaxWidth = 300;
    private const int PreviewMaxWidth   = 1200;
    private const int FullMaxWidth      = 2000;

    public async Task<IReadOnlyList<ImageVariantResult>> GenerateAsync(
        Stream input,
        string originalFileName,
        CancellationToken ct = default)
    {
        if (!input.CanSeek)
        {
            var ms = new MemoryStream();
            await input.CopyToAsync(ms, ct);
            ms.Position = 0;
            input = ms;
        }

        input.Position = 0;

        using var image = await Image.LoadAsync(input, ct);

        var results = new List<ImageVariantResult>(4)
        {
            CreateVariant(image, ImageVariant.Thumbnail, "thumb",   ThumbnailMaxWidth, 75),
            CreateVariant(image, ImageVariant.Preview,   "preview", PreviewMaxWidth,   80),
            CreateVariant(image, ImageVariant.Full,      "full",    FullMaxWidth,      85),
            CreateOriginalVariant(image, ImageVariant.Original, "original", 90)
        };

        return results;
    }

    private static ImageVariantResult CreateVariant(
        Image source,
        ImageVariant variant,
        string suffix,
        int maxWidth,
        int quality)
    {
        using var clone = source.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(maxWidth, 0)
        }));

        var bytes = ToJpegBytes(clone, quality);

        return new ImageVariantResult
        {
            Variant = variant,
            FileNameSuffix = suffix,
            ContentType = "image/jpeg",
            Bytes = bytes
        };
    }

    private static ImageVariantResult CreateOriginalVariant(
        Image source,
        ImageVariant variant,
        string suffix,
        int quality)
    {
        var bytes = ToJpegBytes(source, quality);

        return new ImageVariantResult
        {
            Variant = variant,
            FileNameSuffix = suffix,
            ContentType = "image/jpeg",
            Bytes = bytes
        };
    }

    private static byte[] ToJpegBytes(Image image, int quality)
    {
        using var ms = new MemoryStream();
        image.Save(ms, new JpegEncoder
        {
            Quality = quality
        });
        return ms.ToArray();
    }
    public async Task<IReadOnlyList<ImageVariantResult>> GenerateAvatarAsync(
        Stream input,
        string originalFileName,
        CancellationToken ct = default)
    {
        if (!input.CanSeek)
        {
            var ms = new MemoryStream();
            await input.CopyToAsync(ms, ct);
            ms.Position = 0;
            input = ms;
        }

        input.Position = 0;

        using var image = await Image.LoadAsync(input, ct);

        return new List<ImageVariantResult>
        {
            CreateVariant(image, ImageVariant.AvatarSmall32, "32", 170, 80),
            CreateVariant(image, ImageVariant.AvatarSmall40, "40", 200, 80),
            CreateVariant(image, ImageVariant.AvatarProfile, "profile", 600, 90),
            CreateVariant(image, ImageVariant.AvatarFull, "full", 800, 90),
            CreateOriginalVariant(image, ImageVariant.Original, "original", 90)
        };
    }

}
