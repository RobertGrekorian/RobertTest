
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RobertTest.Services
{
    public class BlobService : IBlobService
    {
        public BlobServiceClient _blobClient { get; }
        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        

        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {
            BlobContainerClient container = _blobClient.GetBlobContainerClient(containerName);
            BlobClient client = container.GetBlobClient(blobName);

            return await client.DeleteIfExistsAsync();
        }

        public async Task<string> GetBlob(string blobName, string containerName)
        {
            BlobContainerClient container = _blobClient.GetBlobContainerClient(containerName);
            BlobClient client = container.GetBlobClient(blobName);

            return client.Uri.AbsoluteUri;
        }


        public async Task<string> UploadBlob(string blobName, string containerName, IFormFile file)
        {
            BlobContainerClient container = _blobClient.GetBlobContainerClient(containerName);
            BlobClient client = container.GetBlobClient(blobName);

            var httpHears = new BlobHttpHeaders()
            {
                ContentType = file.ContentType,
            };
            var result = await client.UploadAsync(file.OpenReadStream(),httpHears);
            if(result != null)
            {
                return await GetBlob(blobName, containerName);   
            }


            return "";

        }
    }
}
