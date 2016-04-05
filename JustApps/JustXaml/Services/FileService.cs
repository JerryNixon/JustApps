using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustXaml.Models;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;
using Windows.Storage.AccessCache;

namespace JustXaml.Services
{
    public class FileService
    {
        FutureService _FutureService;
        JumpListService _JumpListService;

        public static FileService Instance { get; } = new FileService();
        private FileService()
        {
            _FutureService = new FutureService();
            _JumpListService = new JumpListService();
        }

        public async Task<string> ReadTextAsync(File value)
        {
            Uri uri;
            if (!Uri.TryCreate(value.StorageFile.Path, UriKind.Absolute, out uri))
                throw new InvalidCastException(value.StorageFile.Path);
            await _JumpListService.AddAsync(value);
            _FutureService.Add(value);
            return await FileIO.ReadTextAsync(value.StorageFile);
        }
    }
}
