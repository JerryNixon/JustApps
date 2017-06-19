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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace JustD3.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        DataService _DataService = new DataService();
        FavoritesService _FavoritesService = new FavoritesService();

        public ObservableCollection<Group<Session>> Sessions { get; } = new ObservableCollection<Group<Session>>();
        public ObservableCollection<Favorite> Favorites { get; } = new ObservableCollection<Favorite>();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await SetDataAsync(DataService.Sources.Auto);
            ApplicationData.Current.DataChanged += async (s, e) =>
            {
                _FavoritesService.ClearCache();
                await SyncFavoritesAsync();
            };

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            timer.Tick += async (s, e) => await SyncFavoritesAsync();
            timer.Start();
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
            _dataCache = null;
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
                Sessions.Insert(3, new Group<Session> { Title = "11:45 PM Lunch", Brush = brush });
                Sessions.Add(new Group<Session> { Title = "6:00 PM Closing", Brush = brush });
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

            var favs = new List<Favorite>();
            var brush = Colors.DimGray.ToSolidColorBrush();
            var times = sessions.OrderBy(x => x.Date).Select(x => x.Date).Distinct();
            foreach (var time in times)
            {
                var match = sessions.Where(x => x.Date == time && x.IsFavorite).FirstOrDefault();
                if (match == null)
                {
                    favs.Add(new Favorite
                    {
                        Brush = brush,
                        Date = time,
                        Line1 = "No favorite selected",
                        Line2 = "Choose favorites in Sessions",
                    });
                }
                else
                {
                    favs.Add(new Favorite
                    {
                        Brush = brush,
                        Date = time,
                        Line1 = $"{match.Title} by {match.Speaker}",
                        Line2 = match.Room,
                        Session = match,
                    });
                }
            }

            favs.Insert(0, new Favorite
            {
                Date = DateTime.Now.Date.AddHours(8.5),
                Line1 = "Registration",
                Line2 = "General area",
                Brush = brush,
            });
            favs.Insert(3, new Favorite
            {
                Date = DateTime.Now.Date.AddHours(11.75),
                Line1 = "Lunch",
                Line2 = "General area",
                Brush = brush,
            });
            favs.Add(new Favorite
            {
                Date = DateTime.Now.Date.AddHours(18),
                Line1 = "Closing",
                Line2 = "General area",
                Brush = brush,
            });

            var closests = favs.Select(x => new
            {
                Item = x,
                Diff = Math.Abs((DateTime.Now.TimeOfDay - x.Date.TimeOfDay).TotalMinutes),
            });
            var closest = closests.OrderBy(x => x.Diff).First();
            closest.Item.Brush = Colors.OrangeRed.ToSolidColorBrush();

            Favorites.AddRange(favs, true);
        }

        private int DateDiff(DateTime Date1, DateTime Date2)
        {
            TimeSpan time = Date1 - Date2;
            return Math.Abs(time.Minutes);
        }


        #endregion
    }
}

