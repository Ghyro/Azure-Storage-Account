using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorage.Interfaces
{
    public interface IBlobStorageService
    {
        Task<IEnumerable<string>> GetBlobNameListAsync();
        Task<Stream> DownloadDataAsync(string id);
        //Task<string> UploadDataAsync(Stream dataSource, string fileName);  
        Task UploadDataAsync(Stream dataSource, string fileName);
    }
}
