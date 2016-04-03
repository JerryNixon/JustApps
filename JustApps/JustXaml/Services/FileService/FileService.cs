using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;

namespace JustXaml.Services.FileService
{
    public class FileService
    {
        public async Task<IEnumerable<Models.File>> ReadFilesAsync(Models.Folder folder)
        {
            if (folder == null)
                return new Models.File[] { };
            var options = new QueryOptions(CommonFileQuery.DefaultQuery, new[] { ".xaml" });
            var query = folder.StorageFolder.CreateFileQueryWithOptions(options);
            var factory = new FileInformationFactory(query, Windows.Storage.FileProperties.ThumbnailMode.ListView);
            var files = await factory.GetFilesAsync();
            return files.Select(x => new Models.File(x));
        }
    }
}
