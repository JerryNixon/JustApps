using System;
using SQLite;
using WunderlistSdk.Json;

namespace WunderlistSdk.Models
{
    [Table(nameof(Task))]
    internal class Task
    {
        public Task(Json.Task task)
        {
            Id = task.id;
            Title = task.title;
            Starred = task.starred;

            if (DateTime.TryParse(task.due_date, out var dueDate))
            {
                this.DueDate = dueDate;
            }
        }

        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Starred { get; set; }
    }
}
