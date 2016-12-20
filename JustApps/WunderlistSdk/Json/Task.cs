using System;

namespace WunderlistSdk.Json
{
    public class Task
    {
        public int id { get; set; }
        public int assignee_id { get; set; }
        public DateTime created_at { get; set; }
        public int created_by_id { get; set; }
        public string due_date { get; set; }
        public int list_id { get; set; }
        public int revision { get; set; }
        public bool starred { get; set; }
        public string title { get; set; }
    }
}
