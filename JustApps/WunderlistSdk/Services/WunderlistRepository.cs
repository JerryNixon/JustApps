using System;
using System.Collections.Generic;
using System.Linq;
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

        private ServiceLogic _Service = new ServiceLogic();
        public ILogic Service => _Service;

        private DatabaseLogic _Database = new DatabaseLogic();
        public ILogic Database => _Database;
    }

    public class ServiceLogic : ILogic
    {
        internal ServiceHelper Helper;

        public async Task<Models.User> GetUserAsync()
        {
            return await Helper.GetUserAsync();
        }

        public async Task<IEnumerable<Models.Folder>> GetFoldersAsync()
        {
            return await Helper.GetAllFoldersAsync();
        }

        public async Task<IEnumerable<Models.List>> GetListsAsync(Models.Folder folder = null)
        {
            if (folder == null)
            {
                return await Helper.GetAllListsAsync();
            }
            else
            {
                return (await Helper.GetAllListsAsync()).Where(x => folder.Lists.Contains(x.Id));
            }
        }

        public async Task<IEnumerable<Models.Task>> GetTasksAsync(Models.List list)
        {
            return await Helper.GetListTasksAsync(list);
        }
    }

    public class DatabaseLogic : ILogic
    {
        internal DatabaseHelper Helper;

        public async Task<Models.User> GetUserAsync()
        {
            return Helper.Connection.Table<Models.User>().SingleOrDefault();
        }

        public async Task<IEnumerable<Models.Folder>> GetFoldersAsync()
        {
            return Helper.Connection.Table<Models.Folder>();
        }

        public async Task<IEnumerable<Models.List>> GetListsAsync(Models.Folder folder = null)
        {
            // read from database
            if (folder == null)
            {
                return Helper.Connection.Table<Models.List>();
            }
            else
            {
                return Helper.Connection.Table<Models.List>().Where(x => folder.Lists.Contains(x.Id));
            }
        }

        public async Task<IEnumerable<Models.Task>> GetTasksAsync(Models.List list)
        {
            // read from database
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            return Helper.Connection.Table<Models.Task>().Where(x => list.Id == x.ListId);
        }
    }

    public interface ILogic
    {
        Task<Models.User> GetUserAsync();
        Task<IEnumerable<Models.Folder>> GetFoldersAsync();
        Task<IEnumerable<Models.List>> GetListsAsync(Models.Folder folder = null);
        Task<IEnumerable<Models.Task>> GetTasksAsync(Models.List list);
    }
}
