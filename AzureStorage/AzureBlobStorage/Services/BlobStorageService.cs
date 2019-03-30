using AzureBlobStorage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorage.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _containerName = "imagescontainer";
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;

        public BlobStorageService(string storageConnectionString)
        {
            storageConnectionString = storageConnectionString ?? throw new ArgumentNullException($"The {nameof(storageConnectionString)} can not be null.");

            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions = new BlobRequestOptions() { ParallelOperationThreadCount = 4 };
        }

        public async Task<Stream> DownloadDataAsync(string id)
        {
            var blobContainer = _blobClient.GetContainerReference(_containerName);

            var blockBlob = blobContainer.GetBlockBlobReference(id);

            if (await blockBlob.ExistsAsync().ConfigureAwait(false))
            {
                Stream targetStream = new MemoryStream();

                await blockBlob.DownloadToStreamAsync(targetStream).ConfigureAwait(false);

                return targetStream;
            }

            return null;
        }

        public async Task UploadDataAsync(Stream dataSource, string fileName)
        {
            var blobContainer = _blobClient.GetContainerReference(_containerName);

            await blobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null).ConfigureAwait(false);

            var blockBlob = blobContainer.GetBlockBlobReference(fileName);

            using (dataSource)
            {
                await blockBlob.UploadFromStreamAsync(dataSource).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<string>> GetBlobNameListAsync()
        {
            var blobContainer = _blobClient.GetContainerReference(_containerName);

            IEnumerable<string> allBlockBlobs;

            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var resultSegment = await blobContainer.ListBlobsSegmentedAsync(blobContinuationToken).ConfigureAwait(false);

                blobContinuationToken = resultSegment.ContinuationToken;

                allBlockBlobs = resultSegment.Results.Select(i => i is CloudBlockBlob blockBlob ? blockBlob.Name : null).Where(b => b != null);

            } while (blobContinuationToken != null);

            return allBlockBlobs;
        }
    }
}
