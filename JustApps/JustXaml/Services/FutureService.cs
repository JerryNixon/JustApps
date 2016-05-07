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
            {
                futureAccessList.Add(value.StorageFile, value.Metadata);
            }
        }

        public bool Exists(File value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            return futureAccessList.Entries.Any(x => x.Metadata == value.Metadata);
        }

        public async Task<IEnumerable<Models.Folder>> GetFoldersAsync()
        {
            var list = new List<Models.Folder>();
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            var entries = futureAccessList.Entries.Where(x => x.Metadata.StartsWith(typeof(Folder).ToString()));
            foreach (var item in entries)
            {
                var folder = await futureAccessList.GetFolderAsync(item.Token);
                list.Add(new Models.Folder(folder, item.Metadata));
            }
            return list;
        }

        public void Remove(File value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            if (Exists(value))
            {
                var item = futureAccessList.Entries.First(x => x.Metadata == value.Metadata);
                futureAccessList.Remove(item.Token);
            }
        }

        public void Add(Folder value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            if (!Exists(value))
            {
                futureAccessList.Add(value.StorageFolder, value.Metadata);
            }
        }

        public bool Exists(Folder value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            return futureAccessList.Entries.Any(x => x.Metadata == value.Metadata);
        }

        public void Remove(Folder value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            if (Exists(value))
            {
                var item = futureAccessList.Entries.First(x => x.Metadata == value.Metadata);
                futureAccessList.Remove(item.Token);
            }
        }

        internal void Clear()
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            futureAccessList.Clear();
        }

        public async Task<IEnumerable<Models.File>> GetFilesAsync()
        {
            var list = new List<Models.File>();
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            var entries = futureAccessList.Entries.Where(x => x.Metadata.StartsWith(typeof(File).ToString()));
            foreach (var item in entries)
            {
                var file = await futureAccessList.GetFileAsync(item.Token);
                list.Add(new Models.File(file, item.Metadata));
            }
            return list;
        }
    }
}
