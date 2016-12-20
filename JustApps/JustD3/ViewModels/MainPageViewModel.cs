using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Template10.Utils;
using Windows.UI;
using Template10.Common;
using Windows.System;
using JustD3.Models;
using JustD3.Services;
using Windows.Storage;
using Template10.Services.NetworkAvailableService;

namespace JustD3.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        DataService _DataService = new DataService();
        FavoritesService _FavoritesService = new FavoritesService();

        public ObservableCollection<Group<Session>> Sessions { get; } = new ObservableCollection<Group<Session>>();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await SetDataAsync(DataService.Sources.Auto);
            ApplicationData.Current.DataChanged += async (s, e) =>
            {
                _FavoritesService.ClearCache();
                await SyncFavoritesAsync();
            };
        }

        public async void AddFavorite(Session session)
        {
            if (session != null)
            {
                var favorites = await _FavoritesService.AddFavoriteAsync(session);
                await SyncFavoritesAsync(favorites: favorites);
            }
        }

        public async void RemoveFavorite(Session session)
        {
            if (session != null)
            {
                var favorites = await _FavoritesService.RemoveFavoriteAsync(session);
                await SyncFavoritesAsync(favorites: favorites);
            }
        }

        public async void Refresh()
        {
            await SetDataAsync(DataService.Sources.Web);
        }

        public async void Website()
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.denverdevday.com"));
        }

        #region private methods

        Sessions _dataCache;
        private async Task SetDataAsync(DataService.Sources source, Sessions data = null)
        {
            BootStrapper.Current.ModalDialog.IsModal = true;

            data = data ?? _dataCache;
            if (data == null)
            {
                data = await _DataService.GetDataAsync(source);
            }
            _dataCache = data;

            if (data == null)
            {
                // fake placeholders
                Sessions.Add(new Group<Session> { Title = "9:00 to 10:00", Brush = Colors.SteelBlue.ToSolidColorBrush() });
                Sessions.Add(new Group<Session> { Title = "10:15 to 11:15", Brush = Colors.SteelBlue.ToSolidColorBrush() });
                Sessions.Add(new Group<Session> { Title = "11:30 to 12:30", Brush = Colors.SteelBlue.ToSolidColorBrush() });
                Sessions.Add(new Group<Session> { Title = "1:15 to 2:15", Brush = Colors.SteelBlue.ToSolidColorBrush() });
                Sessions.Add(new Group<Session> { Title = "2:30 to 3:30", Brush = Colors.SteelBlue.ToSolidColorBrush() });
                Sessions.Add(new Group<Session> { Title = "3:45 to 4:45", Brush = Colors.SteelBlue.ToSolidColorBrush() });
            }
            else
            {
                var sessions = from s in data.OrderBy(x => x.Date)
                               group s by s.DateFormatted into g
                               select new Group<Session>
                               {
                                   Title = g.Key,
                                   Brush = g.First().Date < DateTime.Now ? Colors.LightSteelBlue.ToSolidColorBrush() : Colors.SteelBlue.ToSolidColorBrush(),
                                   Items = g.OrderBy(x => x.Room)
                               };
                Sessions.AddRange(sessions, true);

                await SyncFavoritesAsync();
            }

            var brush = Colors.DimGray.ToSolidColorBrush();
            try
            {
                Sessions.Insert(0, new Group<Session> { Title = "8:30 AM Registration", Brush = brush });
                Sessions.Insert(4, new Group<Session> { Title = "12:30 PM Lunch", Brush = brush });
                Sessions.Add(new Group<Session> { Title = "5:00 PM Raffle drawing", Brush = brush });
            }
            catch (Exception)
            {
                Sessions.Clear();
                Sessions.Add(new Group<Session> { Title = "Web source data is invalid. Try clicking the refresh button.", Brush = Colors.Pink.ToSolidColorBrush() });
            }

            BootStrapper.Current.ModalDialog.IsModal = false;
        }

        Favorites _favoritesCache;
        private async Task SyncFavoritesAsync(Favorites favorites = null)
        {
            favorites = favorites ?? _favoritesCache;
            if (favorites == null)
            {
                favorites = await _FavoritesService.GetFavoritesAsync();
            }
            _favoritesCache = favorites;

            var sessions = Sessions.Where(x => x.Items != null).SelectMany(x => x.Items);
            foreach (var session in sessions)
            {
                session.IsFavorite = favorites.ContainsValue(session.Id);
            }
        }

        #endregion
    }
}

