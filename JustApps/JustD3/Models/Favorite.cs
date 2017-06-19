using System;
using System.Diagnostics;
using Windows.UI.Xaml.Media;

namespace JustD3.Models
{
    [DebuggerDisplay("{Header}")]
    public class Favorite
    {
        public SolidColorBrush Brush { get; internal set; }
        public string Header => $"{Date.ToString("hh:mm tt")}";
        public string Line1 { get; internal set; }
        public string Line2 { get; internal set; }
        public Session Session { get; internal set; }
        public DateTime Date { get; internal set; }
    }

}