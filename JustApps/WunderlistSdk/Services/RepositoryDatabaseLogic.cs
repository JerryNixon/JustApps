using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WunderlistSdk
{

    public class RepositoryDatabaseLogic : ILogic
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
}
