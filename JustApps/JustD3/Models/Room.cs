using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JustD3.Models
{

    public class Room
    {
        public string id { get; set; }
        public List<object> tags { get; set; }
        public List<object> groups { get; set; }
        public string title { get; set; }
        public int capacity { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public List<object> sessions { get; set; }
        public List<object> sponsors { get; set; }
        public List<Link> links { get; set; }
    }

}