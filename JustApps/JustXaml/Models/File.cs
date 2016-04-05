using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.BulkAccess;

namespace JustXaml.Models
{
    [DebuggerDisplay("{Title} {StorageFolder.Path}")]
    public class File
    {
        public File()
        {
            // empty for design-time
        }

        public File(IStorageFile file) : this()
        {
            StorageFile = file;
            Title = file?.Name;
        }

        public string Title { get; set; }
        public IStorageFile StorageFile { get; set; }

        public async Task<string> ReadTextAsync()
        {
            var service = Services.FileService.Instance;
            return await service.ReadTextAsync(this);
        }

        public static async Task<File> GetAsync(Uri uri)
        {
            if (uri.ToString().StartsWith("ms-"))
            {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                return new File(file);
            }
            else
            {
                var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(uri.ToString());
                return new File(file);
            }
        }
    }
}
