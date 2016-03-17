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
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var root = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var folder = await root.GetFolderAsync("Templates");
            foreach (var subfolder in await folder.GetFoldersAsync())
            {
                var files = await subfolder.GetFilesAsync();
                var group = new Models.Group
                {
                    Title = subfolder.Name,
                    Items = files.Select(x => new Models.Item { Title = x.DisplayName, Path = x.Path }),
                };
                Items.Add(group);
            }
        }

        public ObservableCollection<Models.Group> Items { get; } = new ObservableCollection<Models.Group>();
    }
}

