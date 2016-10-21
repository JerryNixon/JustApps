using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JustD3.Models
{

    public class Session
    {
        public string id { get; set; }
        public string title { get; set; }
        public string speaker { get; set; }
        public string speakerId { get; set; }
        public string @abstract { get; set; }
        public string time { get; set; }
        public string timeFormatted => $"{DateTime.Parse(time).ToString("hh:mm tt")} to {DateTime.Parse(time).AddHours(1).ToString("hh:mm tt")}";
        public string room { get; set; }
        public string roomId { get; set; }
    }

}