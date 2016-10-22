using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustD3.Models
{
    public class Json
    {
        public class Speaker
        {
            public string id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string fullName { get; set; }
            public string company { get; set; }
            public string blog { get; set; }
            public string twitter { get; set; }
            public string bio { get; set; }
            public string imgUrl { get; set; }
        }

        public class Speaker2
        {
            public string id { get; set; }
            public string fullName { get; set; }
        }

        public class Session
        {
            public string id { get; set; }
            public string title { get; set; }
            public List<Speaker2> speakers { get; set; }
            public string @abstract { get; set; }
            public string time { get; set; }
            public string room { get; set; }
            public string roomId { get; set; }
        }

        public class Link
        {
            public string rel { get; set; }
            public string href { get; set; }
        }

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

        public class Sponsor
        {
            public string id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
            public string logo { get; set; }
            public string description { get; set; }
        }

        public class RootObject
        {
            public List<Speaker> speakers { get; set; }
            public List<Session> sessions { get; set; }
            public List<Room> rooms { get; set; }
            public List<Sponsor> sponsors { get; set; }
        }
    }
}
