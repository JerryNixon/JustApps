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
        public static FileService Instance { get; } = new FileService();
        private FileService() { }

        public async Task AddToJumplistAsync(File value)
        {
            if (Windows.UI.StartScreen.JumpList.IsSupported())
            {
                var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
                foreach (var item in jumpList.Items.Skip(4).ToArray())
                {
                    jumpList.Items.Remove(item);
                }
                if (jumpList.Items.Any(x => x.Arguments == value.StorageFile.Path))
                {
                    jumpList.Items.Remove(jumpList.Items.First(x => x.Arguments == value.StorageFile.Path));
                }
                var jumpItem = Windows.UI.StartScreen.JumpListItem.CreateWithArguments(value.StorageFile.Path, value.StorageFile.Name);
                jumpList.Items.Add(jumpItem);
                await jumpList.SaveAsync();
            }
        }

        public void AddToFutureAccessList(File value)
        {
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            futureAccessList.Add(value.StorageFile, value.StorageFile.Path);
        }

        public async Task<string> ReadTextAsync(File value)
        {
            Uri uri;
            if (!Uri.TryCreate(value.StorageFile.Path, UriKind.Absolute, out uri))
                throw new InvalidCastException(value.StorageFile.Path);
            await AddToJumplistAsync(value);
            AddToFutureAccessList(value);
            return await FileIO.ReadTextAsync(value.StorageFile);
        }
    }
}
