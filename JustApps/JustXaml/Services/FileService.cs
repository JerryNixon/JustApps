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
using System.Diagnostics;

namespace JustXaml.Services
{
    public class FileService
    {
        public static FileService Instance { get; } = new FileService();
        private FileService()
        {
            // nothing
        }

        public async Task<File> GetAsync(Uri uri, File.Types type)
        {
            if (uri.ToString().StartsWith("ms-"))
            {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                return new File(file, type);
            }
            else
            {
                var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(uri.ToString());
                return new File(file, type);
            }
        }

        public async Task<string> ReadTextAsync(Uri uri)
        {
            try
            {
                if (uri.ToString().StartsWith("ms-"))
                {
                    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                    return await ReadTextAsync(file);
                }
                else
                {
                    var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(uri.ToString());
                    return await ReadTextAsync(file);
                }
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }

        public async Task<string> ReadTextAsync(IStorageFile file)
        {
            try
            {
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }

        public async Task<string> ReadTextAsync(File value)
        {
            try
            {
            return await ReadTextAsync(value.StorageFile);
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }
    }
}
