using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace JustXaml.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var p = SessionState[parameter.ToString()] as List<Models.Group>;
            p.ForEach(x => Items.Add(x));
            return Task.CompletedTask;
        }

        public ObservableCollection<Models.Group> Items { get; } = new ObservableCollection<Models.Group>();
    }
}

