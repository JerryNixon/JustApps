using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
using Template10.Services.SerializationService;
using System.IO;
using System.Runtime.Serialization.Json;

namespace JustD3.Services
{
    public static class Extensions
    {
        public static Models.RootObject ToRootObject(this string json, DateTime? date = null)
        {
            try
            {
                var _Bytes = Encoding.Unicode.GetBytes(json);
                using (MemoryStream _Stream = new MemoryStream(_Bytes))
                {
                    var _Serializer = new DataContractJsonSerializer(typeof(Models.RootObject));
                    var obj = (Models.RootObject)_Serializer.ReadObject(_Stream);
                    if (date.HasValue)
                    {
                        obj.date = date.Value;
                    }
                    return obj;
                }
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public static Favorites ToFavoritesList(this string json)
        {
            try
            {
                var _Bytes = Encoding.Unicode.GetBytes(json);
                using (MemoryStream _Stream = new MemoryStream(_Bytes))
                {
                    var _Serializer = new DataContractJsonSerializer(typeof(Favorites));
                    return (Favorites)_Serializer.ReadObject(_Stream);
                }
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public static string ToJson(this Models.RootObject obj)
        {
            try
            {
                using (MemoryStream _Stream = new MemoryStream())
                {
                    var _Serializer = new DataContractJsonSerializer(obj.GetType());
                    _Serializer.WriteObject(_Stream, obj);
                    _Stream.Position = 0;
                    using (StreamReader _Reader = new StreamReader(_Stream))
                    {
                        return _Reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

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