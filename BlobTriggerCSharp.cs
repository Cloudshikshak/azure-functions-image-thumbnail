using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Cloudshikshak.Function
{
    public static class BlobTriggerCSharp
    {
        [FunctionName("BlobTriggerCSharp")]
        public static void Run([BlobTrigger("image-input/{name}", Connection = "afcmasterstorageaccount_STORAGE")]Stream inBlob, 
        string name,
        [Blob("image-output/thumbnail-{name}", FileAccess.Write, Connection = "afcmasterstorageaccount_STORAGE")] Stream outBlob,
        ILogger log)
        {
            log.LogInformation($"New image uploaded in image-input container: {name}");
            try
            {
                using (var image = Image.Load(inBlob))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { 
                        Size = new Size { 
                            Height = 100, 
                            Width = 100 }, 
                        Mode = ResizeMode.Crop 
                    }));
        
                    using (var ms = new MemoryStream())
                    {
                        image.SaveAsPng(outBlob);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message, null);
            }
        }
    }
}