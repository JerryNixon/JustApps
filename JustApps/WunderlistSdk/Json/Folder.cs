using System.Collections.Generic;

namespace WunderlistSdk.Json
{
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
}
