using JustXaml.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Diagnostics;

namespace JustXaml.Services
{
    public class FolderService
    {
        public static FolderService Instance { get; } = new FolderService();
        private FolderService()
        {
            // nothing
        }

        public async Task<Models.Folder> PickFolderAsync(string filter = ".xaml")
        {
            var picker = new FolderPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(filter);
            var folder = await picker.PickSingleFolderAsync();
            if (folder == null)
            {
                await new Windows.UI.Popups.MessageDialog("No folder selected.").ShowAsync();
                return null;
            }
            else
            {
                var model = new Models.Folder(folder, Folder.Types.File);
                return model;
            }
        }

        public async Task<IEnumerable<Models.File>> GetFilesAsync(Models.Folder folder, string filter = ".xaml")
        {
            if (folder == null)
            {
                return new Models.File[] { };
            }
            try
            {
                var files = await folder.StorageFolder.GetFilesAsync();
                var type = (folder.Type == Folder.Types.File) ? File.Types.File : File.Types.Sample;
                var result = files.Where(x => System.IO.Path.GetExtension(x.Path) == filter).Select(x => new Models.File(x, type));
                return result;
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }

        public async Task<IEnumerable<Models.Folder>> GetFoldersAsync(Models.Folder folder)
        {
            List<Models.Folder> list = new List<Models.Folder>();
            try
            {
                var folders = await folder.StorageFolder.GetFoldersAsync();
                foreach (var subfolder in folders)
                {
                    list.Add(new Models.Folder(subfolder, folder.Type));
                }
                return list;
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }
    }
}
