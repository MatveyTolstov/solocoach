using Amazon.S3;
using Amazon.S3.Model;

namespace SoloCoachApi.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucket;
        private readonly string _serviceUrl;
        private readonly string _publicUrl;

        public S3Service(IConfiguration configuration)
        {
            var cfg = configuration.GetSection("S3");
            _serviceUrl = cfg["ServiceUrl"]!;
            _bucket = cfg["BucketName"]!;
            _publicUrl = cfg["PublicUrl"]!;;

            var s3Config = new AmazonS3Config
            {
                ServiceURL = _serviceUrl,
                ForcePathStyle = true,
                AuthenticationRegion = cfg["Region"] ?? "ru-1",
                SignatureVersion = "4",
            };

            _client = new AmazonS3Client(cfg["AccessKey"], cfg["SecretKey"], s3Config);
        }

        public async Task<string> UploadAsync(string key, Stream stream, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                UseChunkEncoding = false,
                DisableDefaultChecksumValidation = true,
            };

            await _client.PutObjectAsync(request);
            return $"{_publicUrl}/{key}";
        }

        public async Task DeleteAsync(string key)
        {
            await _client.DeleteObjectAsync(_bucket, key);
        }

        public string ExtractKey(string url)
        {
            var prefix = $"{_publicUrl}/";
            if (url.StartsWith(prefix)) return url[prefix.Length..];
            // fallback для старых URL формата vHosted
            var host = new Uri(_serviceUrl).Host;
            var oldPrefix = $"https://{_bucket}.{host}/";
            return url.StartsWith(oldPrefix) ? url[oldPrefix.Length..] : url;
        }
    }
}
