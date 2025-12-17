using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.RouteBuddy.Common.Services
{
    public class BlobUploadResult
    {
        public string BlobUrl { get; set; } = string.Empty;
        public string BlobName { get; set; } = string.Empty;
    }
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobService(string connectionString, string containerName)
        {
            var client = new BlobServiceClient(connectionString);
            _containerClient = client.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }
        public async Task<BlobUploadResult> UploadFileAsync(IFormFile file)
        {
            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = _containerClient.GetBlobClient(blobName);
            using (var stream = file.OpenReadStream())
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            return new BlobUploadResult
            {
                BlobUrl = blobClient.Uri.ToString(),
                BlobName = blobName
            };
        }
        public async Task DeleteBlobAsync(string? blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName)) return;
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}