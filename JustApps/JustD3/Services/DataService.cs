using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NetworkAvailableService;
using Windows.Storage;
using Windows.UI.Popups;

namespace JustD3.Services
{
    public class DataService
    {
        private readonly Uri uri = new Uri("https://denverdevday.blob.core.windows.net/denverdevday/denverdevdaydata.json");
        private readonly StorageFolder folder = ApplicationData.Current.RoamingFolder;
        private const string name = "data.cache";

        public enum Sources { Web, Cache, Auto }

        public async Task<Models.Sessions> GetDataAsync(Sources source)
        {
            switch (source)
            {
                case Sources.Web:
                    {
                        var net = new NetworkAvailableHelper();
                        if (await net.IsInternetAvailable())
                        {
                            // fetch data
                            var json = await uri.GetStringAsync();
                            var obj = json.Deserialize<Models.Json.RootObject>();

                            // convert to Sessions
                            var projection = obj.sessions.Select(x => new Models.Session
                            {
                                Id = x.id,
                                Room = x.room.Replace("  ", " ").Trim(),
                                Title = x.title.Replace("  ", " ").Trim(),
                                Description = x.@abstract.Replace("  ", " ").Trim(),
                                Date = DateTime.Parse(x.time),
                                Speaker = string.Join(", ", x.speakers.Select(s => s.fullName)).Replace("  ", " ").Trim(),
                            });
                            var sessions = new Models.Sessions(projection);

                            // cache
                            await folder.WriteTextAsync(name, sessions);

                            return sessions;
                        }
                        else
                        {
                            await new MessageDialog("Internet not available").ShowAsync();
                            return null;
                        }
                    }
                case Sources.Cache:
                    {
                        return await folder.ReadTextAsync<Models.Sessions>(name);
                    }
                case Sources.Auto:
                    {
                        var cache = await GetDataAsync(Sources.Cache);
                        if (cache == null || cache.Date < DateTime.Now.AddHours(-1))
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
