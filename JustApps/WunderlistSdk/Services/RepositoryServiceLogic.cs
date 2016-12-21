using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WunderlistSdk
{

    public class RepositoryServiceLogic : ILogic
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
                return (await Helper.GetAllListsAsync()).Where(x => folder.ListsArray.Contains(x.Id));
            }
        }

        public async Task<IEnumerable<Models.Task>> GetTasksAsync(Models.List list)
        {
            return await Helper.GetListTasksAsync(list);
        }
    }
}
