using SQLite;

namespace WunderlistSdk
{
    internal class DatabaseHelper
    {
        public DatabaseHelper()
        {
            var file = $"{nameof(WunderlistRepository)}.sqlite";
            var folder = Windows.Storage.ApplicationData.Current.TemporaryFolder.Path;
            var path = System.IO.Path.Combine(folder, file);
            Connection = new SQLiteConnection(path);
        }

        public SQLiteConnection Connection { get; }

        internal void CreateTables()
        {
            Connection.CreateTable<Models.Folder>();
            Connection.CreateTable<Models.List>();
            Connection.CreateTable<Models.Task>();
            Connection.CreateTable<Models.User>();
        }

        internal void DropTables()
        {
            Connection.DropTable<Models.Folder>();
            Connection.DropTable<Models.List>();
            Connection.DropTable<Models.Task>();
            Connection.DropTable<Models.User>();
        }
    }
}