using JustXaml.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;

namespace JustXaml.Services
{
    public class FutureService
    {
        public void Add(File value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            if (!Exists(value))
                futureAccessList.Add(value.StorageFile, value.StorageFile.Path);
        }

        public bool Exists(File value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            return futureAccessList.Entries.Any(x => x.Metadata == value.StorageFile.Path);
        }

        public async Task<IEnumerable<Models.Folder>> GetFoldersAsync()
        {
            var list = new List<Models.Folder>();
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            foreach (var item in futureAccessList.Entries)
            {
                var folder = await futureAccessList.GetFolderAsync(item.Token);
                list.Add(new Models.Folder(folder));
            }
            return list;
        }

        public void Remove(File value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            if (Exists(value))
            {
                var item = futureAccessList.Entries.First(x => x.Metadata == value.StorageFile.Path);
                futureAccessList.Remove(item.Token);
            }
        }

        public void Add(Folder value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            if (!Exists(value))
                futureAccessList.Add(value.StorageFolder, value.StorageFolder.Path);
        }

        public bool Exists(Folder value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            return futureAccessList.Entries.Any(x => x.Metadata == value.StorageFolder.Path);
        }

        public void Remove(Folder value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            if (Exists(value))
            {
                var item = futureAccessList.Entries.First(x => x.Metadata == value.StorageFolder.Path);
                futureAccessList.Remove(item.Token);
            }
        }

        public async Task<IEnumerable<Models.File>> GetFilesAsync()
        {
            var list = new List<Models.File>();
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            foreach (var item in futureAccessList.Entries)
            {
                var file = await futureAccessList.GetFileAsync(item.Token);
                list.Add(new Models.File(file));
            }
            return list;
        }
    }
}
