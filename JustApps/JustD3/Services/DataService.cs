using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Services.NetworkAvailableService;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace JustD3.Services
{
    public class DataService 
    {
        private readonly Uri uri = new Uri("https://denverdevday.blob.core.windows.net/denverdevday/denverdevdaydata.json");
        private readonly StorageFolder folder = ApplicationData.Current.RoamingFolder;
        private const string name = "data.cache";
        public enum Sources { Web, Cache, Auto }

        public async Task<Models.RootObject> GetDataAsync(Sources source)
        {
            switch (source)
            {
                case Sources.Web:
                    {
                        var net = new NetworkAvailableHelper();
                        if (await net.IsInternetAvailable())
                        {
                            var json = await uri.GetStringAsync();
                            var obj = json.ToRootObject(DateTime.Now);
                            await folder.WriteTextAsync(name, obj.ToJson());
                            return obj;
                        }
                        else
                        {
                            await new MessageDialog("Internet not available").ShowAsync();
                            return null;
                        }
                    }
                case Sources.Cache:
                    {
                        var json = await folder.ReadTextAsync(name);
                        return json.ToRootObject();
                    }
                case Sources.Auto:
                    {
                        var cache = await GetDataAsync(Sources.Cache);
                        if (cache == null || cache.date < DateTime.Now.AddHours(-1))
                        {
                            return await GetDataAsync(Sources.Web);
                        }
                        else
                        {
                            return cache;
                        }
                    }
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
