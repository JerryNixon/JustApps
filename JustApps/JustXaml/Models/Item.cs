using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustXaml.Models
{
    public class Item
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public override string ToString() => Title;
    }
}
