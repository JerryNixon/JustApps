using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JustD3.Services
{
    public class Favorites : Dictionary<DateTime, string> { }
    public class FavoritesService
    {
        private readonly StorageFolder folder = ApplicationData.Current.RoamingFolder;
        private const string name = "favorites.cache";

        public async Task<Favorites> GetFavoritesAsync()
        {
            var json = await folder.ReadTextAsync(name);
            var obj = json.ToFavoritesList();
            return obj ?? new Favorites();
        }

        public async Task<Favorites> AddFavoriteAsync(Models.Session session)
        {
            var favorites = await GetFavoritesAsync();
            if (favorites.ContainsValue(session.id))
            {
                return favorites;
            }
            if (favorites.ContainsKey(session.Date))
            {
                favorites.Remove(session.Date);
            }
            favorites.Add(session.Date, session.id);
            return await SaveAsync(favorites);
        }

        public async Task<Favorites> RemoveFavoriteAsync(Models.Session session)
        {
            var favorites = await GetFavoritesAsync();
            if (!favorites.ContainsValue(session.id))
            {
                return favorites;
            }
            favorites.Remove(session.Date);
            return await SaveAsync(favorites);
        }

        private async Task<Favorites> SaveAsync(Favorites favorites)
        {
            var json = string.Empty;
            using (MemoryStream _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(favorites.GetType());
                _Serializer.WriteObject(_Stream, favorites);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                {
                    json = _Reader.ReadToEnd();
                }
            }
            await folder.WriteTextAsync(name, json);
            return favorites;
        }
    }
}
