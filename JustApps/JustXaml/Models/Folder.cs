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
        public Folder()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Files = Enumerable.Range(1, 10).Select(x => new File { Title = $"File {x}" });
            }
        }

        public Folder(StorageFolder storageFolder) : this()
        {
            StorageFolder = storageFolder;
            Title = storageFolder.Name;
        }

        public string Title { get; set; }
        public IEnumerable<Models.File> Files { get; set; }
        public StorageFolder StorageFolder { get; set; }
        public IEnumerable<Folder> Folders { get; set; }
    }
}
