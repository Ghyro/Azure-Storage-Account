using System.IO;
using Microsoft.Azure.WebJobs;
using System.Drawing;
using System.Drawing.Imaging;
using StackBlur.Extensions;

namespace WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void AddBlurEffect
            ([QueueTrigger("imagequeue")] string Id,
            [Blob("imagescontainer/{queueTrigger}", FileAccess.Read)] Stream inputImage,
            [Blob("imagescontainer/{queueTrigger}", FileAccess.Write)] Stream outputImage)
        {
            using (var bitmap = new Bitmap(inputImage))
            {
                int radius = 100;

                bitmap.StackBlur(radius);

                ImageFormat imgFormat = bitmap.RawFormat;

                bitmap.Save(outputImage, imgFormat);
            }
        }
    }
}
