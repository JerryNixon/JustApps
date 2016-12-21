using System;

namespace WunderlistSdk.Json
{
    public class Task
    {
        public string id { get; set; }
        public string assignee_id { get; set; }
        public DateTime created_at { get; set; }
        public string created_by_id { get; set; }
        public string due_date { get; set; }
        public string list_id { get; set; }
        public string revision { get; set; }
        public bool starred { get; set; }
        public string title { get; set; }
    }
}
