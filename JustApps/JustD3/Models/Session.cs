using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Template10.Mvvm;

namespace JustD3.Models
{
    [DebuggerDisplay("{title}")]
    public class Session : BindableBase
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<Speaker2> speakers { get; set; }
        public string @abstract { get; set; }
        public string time { get; set; }
        public string room { get; set; }
        public string roomId { get; set; }

        public string speaker => string.Join(", ", speakers?.Select(x => x.fullName.Replace("  ", " ")) ?? new[] { "Unknown" });
        public string timeFormatted => $"{Date.ToString("hh:mm tt")} to {Date.AddHours(1).ToString("hh:mm tt")}";
        public DateTime Date => System.DateTime.Parse(time);

        bool _IsFavorite = default(bool);
        public bool IsFavorite
        {
            get { return _IsFavorite; }
            set
            {
                Set(ref _IsFavorite, value);
            }
        }
    }
}