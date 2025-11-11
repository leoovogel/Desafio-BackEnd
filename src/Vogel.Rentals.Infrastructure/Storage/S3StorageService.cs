using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Vogel.Rentals.Application.Abstractions;

namespace Vogel.Rentals.Infrastructure.Storage;

public class S3StorageService : IStorageService, IDisposable
{
    private readonly IAmazonS3 _s3;
    private readonly S3Options _options;

    public S3StorageService(IOptions<S3Options> options)
    {
        _options = options.Value;

        _s3 = new AmazonS3Client(RegionEndpoint.GetBySystemName(_options.Region));
    }

    public async Task<string> SaveAsync(string fileName, byte[] content)
    {
        using var stream = new MemoryStream(content);

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = fileName,
            InputStream = stream
        };

        await _s3.PutObjectAsync(request);
        return fileName;
    }

    public void Dispose()
    {
        _s3.Dispose();
    }
}