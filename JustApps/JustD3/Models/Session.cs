using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Template10.Mvvm;

namespace JustD3.Models
{
    [DebuggerDisplay("{Summary}")]
    public class Session : BindableBase
    {
        public String Id { get; internal set; }
        public String Title { get; internal set; }
        public String Description { get; internal set; }
        public String Room { get; internal set; }
        public String Speaker { get; internal set; }
        public DateTime Date { get; internal set; }
        public string DateFormatted => FormatDate(Date);
        public string Summary => $"{Title} by {Speaker} in {Room}";

        public static string FormatDate(DateTime date) => $"{date.ToString("hh:mm tt")} to {date.AddMinutes(75).ToString("hh:mm tt")}";

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