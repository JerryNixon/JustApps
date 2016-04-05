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
        public File()
        {
        }

        public File(IStorageFile file) : this()
        {
            StorageFile = file;
            Title = file?.Name;
        }

        public string Title { get; set; }
        public IStorageFile StorageFile { get; set; }
    }
}
