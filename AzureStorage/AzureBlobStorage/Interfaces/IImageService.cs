using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorage.Interfaces
{
    public interface IImageService
    {
        Task<IEnumerable<string>> GetAllImagesNameAsync();
        Task<Stream> GetImageAsync(string imageName);
        Task AddImageAsync(Stream file, string imageName);
    }
}
