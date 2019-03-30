using AzureBlobStorage.Interfaces;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorage.Services
{
    public class ImageService : IImageService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueStorageService _queueStorageService;
        private readonly Logger _logger;

        public ImageService(IBlobStorageService blobStorageService, IQueueStorageService queueStorageService)
        {
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _queueStorageService = queueStorageService ?? throw new ArgumentNullException(nameof(queueStorageService));  
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<string>> GetAllImagesNameAsync()
        {
            _logger.Info($"Loading all images name from storage.");

            var result = await _blobStorageService.GetBlobNameListAsync();

            return result;
        }

        public async Task<Stream> GetImageAsync(string imageName)
        {
            _logger.Info($"Loading image {imageName} from storage.");

            var dataStream = await _blobStorageService.DownloadDataAsync(imageName).ConfigureAwait(false);

            if (dataStream == null)
            {
                _logger.Error($"Image {imageName} not found.");
            }

            _logger.Info($"Image {imageName} is loaded from storage.");

            return dataStream;
        }

        public async Task AddImageAsync(Stream file, string imageName)
        {
            _logger.Info($"Start uploading image {imageName} to storage.");

            await _blobStorageService.UploadDataAsync(file, imageName).ConfigureAwait(false);

            string messageId = await _queueStorageService.EnqueueMessageAsync(imageName).ConfigureAwait(false);

            _logger.Info($"Image {imageName} is uploaded to storage.");
        }
    }
}
