using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustD3.Models
{
    public class RootObject
    {
        public DateTime date { get; set; }
        public List<Speaker> speakers { get; set; }
        public List<Session> sessions { get; set; }
        public List<Room> rooms { get; set; }
        public List<Sponsor> sponsors { get; set; }
    }
}
