using System;
using System.Threading.Tasks;

namespace WunderlistSdk
{
    public class WunderlistRepository
    {
        public WunderlistRepository(WunderlistSettings settings)
        {
            _Service.Helper = new ServiceHelper(settings);
            _Database.Helper = new DatabaseHelper();
            _Database.Helper.CreateTables();
        }

        public async Task<bool> AuthenticateAsync()
        {
            _Service.Helper.User = await Database.GetUserAsync();
            return await _Service.Helper.AuthenticateAsync();
        }

        public async Task<bool> RefreshDatabaseAsync()
        {
            if (!await AuthenticateAsync())
            {
                return false;
            }

            // clear existing data
            _Database.Helper.DropTables();
            _Database.Helper.CreateTables();

            var folders = await _Service.Helper.GetAllFoldersAsync();
            _Database.Helper.Connection.InsertAll(folders);

            var lists = await _Service.Helper.GetAllListsAsync();
            _Database.Helper.Connection.InsertAll(lists);

            var tasks = await _Service.Helper.GetListTasksAsync(lists);
            _Database.Helper.Connection.InsertAll(tasks);

            return true;
        }

        private RepositoryServiceLogic _Service = new RepositoryServiceLogic();
        public ILogic Service => _Service;

        private RepositoryDatabaseLogic _Database = new RepositoryDatabaseLogic();
        public ILogic Database => _Database;
    }
}
