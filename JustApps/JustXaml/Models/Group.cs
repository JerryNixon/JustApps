using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustXaml.Models
{
    public class Group
    {
        public string Title { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public override string ToString() => Title;
    }
}
