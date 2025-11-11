namespace Vogel.Rentals.Infrastructure.Storage;

public class S3Options
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "sa-east-1";
}