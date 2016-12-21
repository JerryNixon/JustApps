using System;
using SQLite;
using WunderlistSdk.Json;

namespace WunderlistSdk.Models
{
    [Table(nameof(Task))]
    public class Task
    {
        public Task()
        {
            // for SQLite
        }
        internal Task(Json.Task task)
        {
            Id = task.id;
            Title = task.title;
            Starred = task.starred;
            Revision = task.revision;
            ListId = task.list_id;

            if (DateTime.TryParse(task.due_date, out var dueDate))
            {
                this.DueDate = dueDate;
            }
        }

        [PrimaryKey]
        public int Id { get; set; }
        public int ListId { get; set; }
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Starred { get; set; }
        public int Revision { get; set; }

        public async System.Threading.Tasks.Task ShowAsync()
        {
            var uri = new Uri($"https://www.wunderlist.com/#/tasks/{Id}/title/focus");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
