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

namespace JustD3.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        DataService _DataService;
        FavoritesService _FavoritesService;

        public MainPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _DataService = new DataService();
                _FavoritesService = new FavoritesService();
            }
        }

        public ObservableCollection<Group<Session>> Sessions { get; } = new ObservableCollection<Group<Session>>();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await SetDataAsync(DataService.Sources.Auto);
        }

        public async void AddFavorite(Session session)
        {
            if (session == null) return;
            var favorites = await _FavoritesService.AddFavoriteAsync(session);
            await SyncFavoritesAsync(favorites: favorites);
        }

        public async void RemoveFavorite(Session session)
        {
            if (session == null) return;
            var favorites = await _FavoritesService.RemoveFavoriteAsync(session);
            await SyncFavoritesAsync(favorites: favorites);
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

        RootObject _dataCache;
        private async Task SetDataAsync(DataService.Sources source, RootObject data = null)
        {
            BootStrapper.Current.ModalDialog.IsModal = true;

            data = data ?? _dataCache;
            if (data == null)
            {
                data = await _DataService.GetDataAsync(source);
            }
            _dataCache = data;

            var sessions = from s in data.sessions.OrderBy(x => x.Date)
                           group s by s.timeFormatted into g
                           select new Group<Session>
                           {
                               Title = g.Key,
                               Brush = g.First().Date < DateTime.Now
                                ? Colors.LightSteelBlue.ToSolidColorBrush()
                                : Colors.SteelBlue.ToSolidColorBrush(),
                               Items = g.OrderBy(x => x.room)
                           };
            Sessions.AddRange(sessions, true);

            await SyncFavoritesAsync();

            var brush = Colors.DimGray.ToSolidColorBrush();
            Sessions.Insert(0, new Group<Session> { Title = "8:30 AM Morning Registration", Brush = brush });
            Sessions.Insert(4, new Group<Session> { Title = "12:30 PM Mid-day Lunch", Brush = brush });
            Sessions.Add(new Group<Session> { Title = "5:00 PM End of day Raffle", Brush = brush });

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
                session.IsFavorite = favorites.ContainsValue(session.id);
            }
        }

        #endregion
    }
}

