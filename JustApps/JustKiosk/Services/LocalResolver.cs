using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustKiosk.Models;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace JustKiosk.Services
{
    public sealed class LocalResolver : Windows.Web.IUriToStreamResolver
    {
        private FolderInfo folder;
        public LocalResolver(FolderInfo folder)
        {
            this.folder = folder;
        }

        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            return GetContentAsync(uri).AsAsyncOperation();
        }

        private async Task<IInputStream> GetContentAsync(Uri relativePath)
        {
            try
            {
                var clean = relativePath.AbsolutePath.Replace("/", "\\");
                var path = $"{folder.StorageFolder.Path}{clean}";
                var file = await StorageFile.GetFileFromPathAsync(path);
                var stream = await file.OpenAsync(FileAccessMode.Read);
                return stream.GetInputStreamAt(0);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
