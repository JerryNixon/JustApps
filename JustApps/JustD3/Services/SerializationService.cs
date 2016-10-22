using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace JustD3.Services
{
    public static class SerializationService
    {
        public static string Serialize<T>(T obj, bool throwOnError = false)
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

        public static T Deserialize<T>(string json, bool throwOnError = false)
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
