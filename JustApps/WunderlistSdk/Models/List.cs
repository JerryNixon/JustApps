using System;
using SQLite;
using WunderlistSdk.Json;

namespace WunderlistSdk.Models
{

    [Table(nameof(List))]
    public class List
    {
        public List()
        {
            // for SQLite
        }
        internal List(Json.List list)
        {
            Id = list.id;
            Title = list.title;
        }

        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }

        public async System.Threading.Tasks.Task ShowAsync()
        {
            var uri = new Uri($"https://www.wunderlist.com/#/lists/{Id}");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
