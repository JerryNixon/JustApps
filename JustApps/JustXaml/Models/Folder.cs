using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustXaml.Models
{
    public class Folder
    {
        public string Title { get; set; }
        public IEnumerable<Models.File> Files { get; set; }
    }
}
