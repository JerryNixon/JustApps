using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JustXaml.Models
{
    [DebuggerDisplay("{Title}")]
    public class Folder
    {
        private StorageFolder folder;

        public enum Types { File, Sample }
        public Types Type { get; set; }

        public Folder()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Files = Enumerable.Range(1, 10).Select(x => new File { Title = $"File {x}" });
            }
        }

        public Folder(StorageFolder storageFolder, Types type) : this()
        {
            try
            {
                Type = type;
                StorageFolder = storageFolder;
                Title = storageFolder.Name;
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }

        public Folder(StorageFolder folder, string metadata) : this(folder, new MetadataInfo(metadata).Type)
        {
            // handled in chain
        }

        #region Metadata

        public string Metadata => new MetadataInfo(Type, StorageFolder.Path).ToString();

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
                catch (Exception ex)
                {
                    Debugger.Break();
                    throw;
                }
            }
            public Types Type { get; set; }
            public Uri Path { get; set; }
            public override string ToString()
            {
                return $"{typeof(Folder)}|{Type}|{Path}";
            }
        }

        #endregion

        public string Title { get; set; }
        public IEnumerable<Models.File> Files { get; set; }
        public StorageFolder StorageFolder { get; set; }
        public IEnumerable<Folder> Folders { get; set; }
    }
}
