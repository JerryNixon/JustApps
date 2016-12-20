using SQLite;
using WunderlistSdk.Json;

namespace WunderlistSdk.Json
{
}

namespace WunderlistSdk.Models
{

    [Table(nameof(List))]
    internal class List
    {
        public List(Json.List list)
        {
            Id = list.id;
            Title = list.title;
        }

        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
