using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace WunderlistSdk.Models
{
    [Table(nameof(Folder))]
    public class Folder
    {
        public Folder()
        {
            // for SQLite
        }
        internal Folder(Json.Folder folder)
        {
            Id = folder.id;
            Title = folder.title;
            Lists = string.Join(",", folder.list_ids);
        }

        [PrimaryKey]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Lists { get; set; }
        public string[] ListsArray => Lists.Split(',').ToArray();
    }
}
