using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;

namespace JustD3.Services
{
    public static class Extensions
    {
        static HttpClient httpClient = new HttpClient();
        public static async Task<string> GetStringAsync(this Uri uri)
        {
            try
            {
                return await httpClient.GetStringAsync(uri);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<string> ReadTextAsync(this StorageFolder folder, string name)
        {
            var file = await folder.TryGetItemAsync(name) as StorageFile;
            try
            {
                return await FileIO.ReadTextAsync(file);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<StorageFile> WriteTextAsync(this StorageFolder folder, string name, string text)
        {
            var file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, text);
            return file;
        }
    }

}