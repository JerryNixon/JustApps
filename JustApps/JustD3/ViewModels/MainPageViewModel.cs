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

namespace JustD3.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        Services.IDataService _DataService;

        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _DataService = new Design.DataService();
                Data = _DataService.GetDataAsync(Services.DataService.Sources.Auto).Result;
            }
            else
            {
                _DataService = new Services.DataService();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            try
            {
                BootStrapper.Current.ModalDialog.IsModal = true;
                Data = await _DataService.GetDataAsync(Services.DataService.Sources.Auto);
            }
            finally
            {
                BootStrapper.Current.ModalDialog.IsModal = false;
            }

        }

        Models.RootObject Data
        {
            set
            {
                var sessions = from s in value.sessions.OrderBy(x => x.time)
                               group s by s.timeFormatted into g
                               select new Models.Group<Models.Session>
                               {
                                   Title = g.Key,
                                   Brush = DateTime.Parse(g.First().time) < DateTime.Now ? Colors.LightSteelBlue.ToSolidColorBrush() : Colors.SteelBlue.ToSolidColorBrush(),
                                   Items = g.OrderBy(x => x.room)
                               };
                Sessions.AddRange(sessions, true);
                var brush = Colors.DimGray.ToSolidColorBrush();
                Sessions.Insert(0, new Models.Group<Models.Session> { Title = "8:30 AM Morning Registration", Brush = brush });
                Sessions.Insert(4, new Models.Group<Models.Session> { Title = "12:30 PM Mid-day Lunch", Brush = brush });
                Sessions.Add(new Models.Group<Models.Session> { Title = "5:00 PM End of day Raffle", Brush = brush });
            }
        }

        public ObservableCollection<Models.Group<Models.Session>> Sessions { get; } = new ObservableCollection<Models.Group<Models.Session>>();
    }
}

