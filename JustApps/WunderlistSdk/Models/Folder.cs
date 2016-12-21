using System.Collections.Generic;
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
            Lists = folder.list_ids.ToArray();
        }

        [PrimaryKey]
        public int Id { get; }
        public string Title { get; }
        public int[] Lists { get; }
    }
}
