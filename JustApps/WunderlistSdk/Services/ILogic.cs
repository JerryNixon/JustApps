using System.Collections.Generic;
using System.Threading.Tasks;

namespace WunderlistSdk
{

    public interface ILogic
    {
        Task<Models.User> GetUserAsync();
        Task<IEnumerable<Models.Folder>> GetFoldersAsync();
        Task<IEnumerable<Models.List>> GetListsAsync(Models.Folder folder = null);
        Task<IEnumerable<Models.Task>> GetTasksAsync(Models.List list);
    }
}
