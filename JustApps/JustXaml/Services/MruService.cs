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
        #region Folders

        public async Task<IEnumerable<Models.Folder>> GetFoldersAsync(Folder.Types? type)
        {
            var list = new List<Models.Folder>();
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var entries = mru.Entries.Where(x => x.Metadata.StartsWith(typeof(Folder).ToString()));
            if (type != null)
            {
                entries = entries.Where(x => new Folder.MetadataInfo(x.Metadata).Type == type);
            }
            foreach (var item in entries)
            {
                var Folder = await mru.GetFolderAsync(item.Token);
                list.Add(new Models.Folder(Folder, item.Metadata));
            }
            return list;
        }

        public void Add(Folder value, int limit = 10)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            RemoveExisting(value);
            PruneListToLimit(limit, value.Type);
            mru.Add(value.StorageFolder, value.Metadata);
        }

        private static void RemoveExisting(Folder value)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var item in mru.Entries.Where(x => x.Metadata == value.Metadata).ToArray())
            {
                mru.Remove(item.Token);
            }
        }

        public bool Exists(Folder value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return mru.Entries.Any(x => x.Metadata == value.Metadata);
        }

        public void Clear(Folder.Types? type)
        {
            try
            {
                var mru = StorageApplicationPermissions.MostRecentlyUsedList;
                var entries = mru.Entries.Where(x => x.Metadata.StartsWith(typeof(Folder).ToString()));
                if (type != null)
                {
                    entries = entries.Where(x => new Folder.MetadataInfo(x.Metadata).Type == type);
                }
                foreach (var item in entries.ToArray())
                {
                    mru.Remove(item.Token);
                }
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

        private static void PruneListToLimit(int limit, Folder.Types? type)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var entries = mru.Entries.Where(x => x.Metadata.StartsWith(typeof(Folder).ToString()));
            if (type != null)
            {
                entries = entries.Where(x => new Folder.MetadataInfo(x.Metadata).Type == type);
            }
            foreach (var item in entries.Skip(limit - 1).ToArray())
            {
                mru.Remove(item.Token);
            }
        }

        #endregion

        #region files

        public async Task<IEnumerable<Models.File>> GetFilesAsync(File.Types? type = null)
        {
            var list = new List<Models.File>();
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var entries = mru.Entries.Where(x => x.Metadata.StartsWith(typeof(File).ToString()));
            if (type != null)
                entries = entries.Where(x => new File.MetadataInfo(x.Metadata).Type == type);
            foreach (var item in entries)
            {
                var file = await mru.GetFileAsync(item.Token);
                list.Add(new File(file, item.Metadata));
            }
            return list;
        }

        public void Add(File value, int limit = 4)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            RemoveExisting(value);
            PruneListToLimit(limit, value.Type);
            mru.Add(value.StorageFile, value.Metadata);
        }

        private static void RemoveExisting(File value)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var item in mru.Entries.Where(x => x.Metadata == value.Metadata).ToArray())
            {
                mru.Remove(item.Token);
            }
        }

        public bool Exists(File value, string metadata = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return mru.Entries.Any(x => x.Metadata == (value.Metadata));
        }

        public void Clear(File.Types type)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var entries = mru.Entries.Where(x => x.Metadata.StartsWith(typeof(File).ToString()));
            entries = entries.Where(x => new File.MetadataInfo(x.Metadata).Type == type);
            foreach (var item in entries.ToArray())
            {
                mru.Remove(item.Token);
            }
        }

        private static void PruneListToLimit(int limit, File.Types? type)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var entries = mru.Entries.Where(x => x.Metadata.StartsWith(typeof(File).ToString()));
            if (type != null)
                entries = entries.Where(x => new File.MetadataInfo(x.Metadata).Type == type);
            foreach (var item in entries.Skip(limit - 1).ToArray())
            {
                mru.Remove(item.Token);
            }
        }

        #endregion
    }
}
