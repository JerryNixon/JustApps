using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustD3.Models;
using JustD3.Services;

namespace JustD3.Design
{
    public class DataService : IDataService
    {
        public async Task<RootObject> GetDataAsync(Services.DataService.Sources source)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            folder = await folder.GetFolderAsync("Design");
            var json = await folder.ReadTextAsync("data.json");
            return json.ToRootObject(DateTime.Now);
        }
    }
}
