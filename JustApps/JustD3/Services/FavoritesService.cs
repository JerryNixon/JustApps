﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JustD3.Services
{
    public class FavoritesService
    {
        private readonly StorageFolder folder = ApplicationData.Current.RoamingFolder;
        private const string name = "favorites.cache";

        public void ClearCache() => _cache = null;

        Models.Favorites _cache;
        public async Task<Models.Favorites> GetFavoritesAsync()
        {
            if (_cache != null)
            {
                return _cache;
            }
            var json = await folder.ReadTextAsync(name);
            Models.Favorites obj = null;
            try
            {
                _cache = obj = json.Deserialize<Models.Favorites>();
            }
            catch { }
            return obj ?? new Models.Favorites();
        }

        public async Task<Models.Favorites> AddFavoriteAsync(Models.Session session)
        {
            var favorites = await GetFavoritesAsync();
            if (favorites.ContainsValue(session.Id))
            {
                return favorites;
            }
            if (favorites.ContainsKey(session.Date))
            {
                favorites.Remove(session.Date);
            }
            favorites.Add(session.Date, session.Id);
            return await SaveAsync(favorites);
        }

        public async Task<Models.Favorites> RemoveFavoriteAsync(Models.Session session)
        {
            var favorites = await GetFavoritesAsync();
            if (!favorites.ContainsValue(session.Id))
            {
                return favorites;
            }
            favorites.Remove(session.Date);
            return await SaveAsync(favorites);
        }

        private async Task<Models.Favorites> SaveAsync(Models.Favorites favorites)
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
