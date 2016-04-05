using JustXaml.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;

namespace JustXaml.Services
{
    public class MruService
    {
        FutureService _FutureService;

        public MruService()
        {
            _FutureService = new FutureService();
        }

        public async Task<IEnumerable<Models.Folder>> GetFoldersAsync(string metadata = null)
        {
            var list = new List<Models.Folder>();
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var item in mru.Entries.Where(x => x.Metadata == (metadata ?? x.Metadata)))
            {
                var Folder = await mru.GetFolderAsync(item.Token);
                list.Add(new Models.Folder(Folder));
            }
            return list;
        }

        public void Add(Folder value, string metadata = null, int limit = 10)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var item in mru.Entries.Where(x => x.Metadata == metadata).Skip(limit - 1).ToArray())
            {
                mru.Remove(item.Token);
            }
            if (!Exists(value))
                mru.Add(value.StorageFolder, metadata ?? value.StorageFolder.Path);
            _FutureService.Add(value);
        }

        public bool Exists(Folder value, string metadata = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return mru.Entries.Any(x => x.Metadata == (metadata ?? value.StorageFolder.Path));
        }

        public void Clear()
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            mru.Clear();
        }

        public async Task<IEnumerable<Models.File>> GetFilesAsync(string metadata = null)
        {
            var list = new List<Models.File>();
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var item in mru.Entries.Where(x => x.Metadata == (metadata ?? x.Metadata)))
            {
                try
                {
                    var file = await mru.GetFileAsync(item.Token);
                    list.Add(new Models.File(file));
                }
                catch (Exception)
                {
                    Debugger.Break();
                }
            }
            return list;
        }

        public void Add(File value, string metadata = null, int limit = 10)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var item in mru.Entries.Where(x => x.Metadata == metadata).Skip(limit - 1).ToArray())
            {
                mru.Remove(item.Token);
            }
            if (!Exists(value))
                mru.Add(value.StorageFile, metadata ?? value.StorageFile.Path);
            _FutureService.Add(value);
        }

        public bool Exists(File value, string metadata = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return mru.Entries.Any(x => x.Metadata == (metadata ?? value.StorageFile.Path));
        }




    }
}
