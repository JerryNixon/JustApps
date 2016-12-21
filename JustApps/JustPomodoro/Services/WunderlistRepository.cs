using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace JustPomodoro.Services
{
    public class WunderlistRepository
    {
        private readonly SQLiteConnection connection;

        public WunderlistRepository()
        {
            var file = "Wunderlist.db3";
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            var path = System.IO.Path.Combine(folder, file);
            connection = new SQLiteConnection(path);
            CreateTables();
        }
    }
}
