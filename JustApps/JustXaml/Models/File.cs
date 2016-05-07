using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.BulkAccess;

namespace JustXaml.Models
{
    [DebuggerDisplay("{Title} {StorageFolder.Path}")]
    public class File
    {
        private StorageFile file;
        private string metadata;

        public File()
        {
            // empty for design-time
        }

        public enum Types { File, Sample }
        public Types Type { get; set; }

        public File(IStorageFile file, Types type) : this()
        {
            try
            {
                Type = type;
                StorageFile = file;
                Title = System.IO.Path.GetFileNameWithoutExtension(file?.Name);
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }

        public File(StorageFile file, string metadata) : this(file, new MetadataInfo(metadata).Type)
        {
            // empty, handled in chain
        }

        #region Metadata

        public string Metadata => new MetadataInfo(Type, StorageFile.Path).ToString();

        public class MetadataInfo
        {
            public MetadataInfo(Types type, string path)
            {
                Type = type;
                Path = new Uri(path);
            }
            public MetadataInfo(string metadata)
            {
                try
                {
                    var array = metadata.Split('|');
                    var type = array[1];
                    Type = (Types)Enum.Parse(typeof(Types), type);
                    var path = array[2];
                    Path = new Uri(path);
                }
                catch
                {
                    Debugger.Break();
                    throw;
                }
            }
            public Types Type { get; set; }
            public Uri Path { get; set; }
            public override string ToString()
            {
                return $"{typeof(File)}|{Type}|{Path}";
            }
        }

        #endregion

        public string Title { get; set; }
        public IStorageFile StorageFile { get; set; }
    }
}
