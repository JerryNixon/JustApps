using System.Threading.Tasks;
using JustD3.Models;

namespace JustD3.Services
{
    public interface IDataService
    {
        Task<RootObject> GetDataAsync(DataService.Sources source);
    }
}