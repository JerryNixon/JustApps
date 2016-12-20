using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustPomodoro.Services.Json
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string created_at { get; set; }
        public int revision { get; set; }
    }

    public class Folder
    {
        public int id { get; set; }
        public string title { get; set; }
        public int user_id { get; set; }
        public List<int> list_ids { get; set; }
        public string created_at { get; set; }
        public int created_by_id { get; set; }
        public string created_by_request_id { get; set; }
        public string updated_at { get; set; }
        public string type { get; set; }
        public int revision { get; set; }
    }

    public class List
    {
        public int id { get; set; }
        public string created_at { get; set; }
        public string title { get; set; }
        public string list_type { get; set; }
        public string type { get; set; }
        public int revision { get; set; }
    }
}
