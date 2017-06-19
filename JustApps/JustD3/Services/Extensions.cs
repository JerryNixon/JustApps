namespace JustD3.Services
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Web.Http;

    public static class Extensions
    {
        static HttpClient httpClient = new HttpClient();
        public static async Task<string> GetStringAsync(this Uri uri, bool throwOnError = false)
        {
            try
            {
                return await httpClient.GetStringAsync(uri);
            }
            catch
            {
                if (throwOnError) throw;
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

        public static async Task<T> ReadTextAsync<T>(this StorageFolder folder, string name)
        {
            try
            {
                var json = await folder.ReadTextAsync(name);
                return json.Deserialize<T>(true);
            }
            catch
            {
                return default(T);
            }
        }

        public static async Task<StorageFile> WriteTextAsync(this StorageFolder folder, string name, string text)
        {
            var file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, text);
            return file;
        }

        public static async Task<StorageFile> WriteTextAsync<T>(this StorageFolder folder, string name, T value)
        {
            var json = value.Serialize(true);
            return await folder.WriteTextAsync(name, json);
        }

        public static string Serialize<T>(this T obj, bool throwOnError = false)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    serializer.WriteObject(stream, obj);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                if (throwOnError) throw;
                return String.Empty;
            }
        }

        public static T Deserialize<T>(this string json, bool throwOnError = false)
        {
            try
            {
                var _Bytes = Encoding.Unicode.GetBytes(json);
                using (MemoryStream _Stream = new MemoryStream(_Bytes))
                {
                    var _Serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)_Serializer.ReadObject(_Stream);
                }
            }
            catch
            {
                if (throwOnError) throw;
                return default(T);
            }
        }
    }
}