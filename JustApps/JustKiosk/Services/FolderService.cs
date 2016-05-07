using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustKiosk.Models;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace JustKiosk.Services
{
    public class FolderService
    {
        string FolderKey = nameof(FolderInfo);
        IPropertySet localSettings = ApplicationData.Current.LocalSettings.Values;
        StorageItemAccessList futureAccessList = StorageApplicationPermissions.FutureAccessList;

        public async Task<FolderInfo> PickFolderAsync()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            folderPicker.FileTypeFilter.Add(".nixon");
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder == null)
            {
                var dialog = new MessageDialog("No folder selected");
                await dialog.ShowAsync();
                return null;
            }
            else
            {
                var token = futureAccessList.Add(folder);
                localSettings[FolderKey] = token;
                return folder.ToFolderInfo();
            }
        }

        public async Task<FolderInfo> RememberFolderAsync()
        {
            if (localSettings.ContainsKey(FolderKey))
            {
                var token = localSettings[FolderKey].ToString();
                var folder = await futureAccessList.GetFolderAsync(token);
                return folder.ToFolderInfo();
            }
            else
            {
                return null;
            }
        }
    }
}
