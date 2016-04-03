using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.BulkAccess;

namespace JustXaml.Models
{
    public class File
    {
        public File()
        {
        }

        public File(FileInformation fileInformation) : this()
        {
            FileInformation = fileInformation;
        }

        public string Title { get; set; }
        public FileInformation FileInformation { get; set; }
    }
}
