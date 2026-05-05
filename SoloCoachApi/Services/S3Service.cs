using Amazon.S3;
using Amazon.S3.Model;

namespace SoloCoachApi.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucket;
        private readonly string _serviceUrl;

        public S3Service(IConfiguration configuration)
        {
            var cfg = configuration.GetSection("S3");
            _serviceUrl = cfg["ServiceUrl"]!;
            _bucket = cfg["BucketName"]!;

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
            
            // virtual-hosted style: https://{bucket}.s3.storage.selcloud.ru/{key}
            var host = new Uri(_serviceUrl).Host;
            return $"https://{_bucket}.{host}/{key}";
        }

        public async Task DeleteAsync(string key)
        {
            await _client.DeleteObjectAsync(_bucket, key);
        }

        public string ExtractKey(string url)
        {
            var host = new Uri(_serviceUrl).Host;
            var prefix = $"https://{_bucket}.{host}/";
            return url.StartsWith(prefix) ? url[prefix.Length..] : url;
        }
    }
}
