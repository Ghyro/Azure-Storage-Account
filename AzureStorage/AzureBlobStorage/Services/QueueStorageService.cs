using AzureBlobStorage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorage.Services
{
    public class QueueStorageService : IQueueStorageService
    {
        private readonly string _queueName = "imagequeue";
        private CloudStorageAccount _storageAccount;
        private CloudQueueClient _queueClient;

        public QueueStorageService(string storageConnectionString)
        {
            storageConnectionString = storageConnectionString ?? throw new ArgumentNullException(nameof(storageConnectionString));

            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _queueClient = _storageAccount.CreateCloudQueueClient();
        }

        public async Task<string> EnqueueMessageAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException($"The {message} can not be null or empty.");
            }

            var cloudQueue = _queueClient.GetQueueReference(_queueName);

            await cloudQueue.CreateIfNotExistsAsync().ConfigureAwait(false);

            var queueMessage = new CloudQueueMessage(message);

            await cloudQueue.AddMessageAsync(queueMessage).ConfigureAwait(false);

            return queueMessage.Id;
        }
    }
}
