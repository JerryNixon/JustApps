using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace JustD3.Models
{
    public class Group<T>
    {
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public IEnumerable<T> Items { get; set; }
        public Brush Brush { get; set; }
    }
}
