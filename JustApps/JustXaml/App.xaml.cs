using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace JustXaml
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        public App() { InitializeComponent(); }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            var root = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var folder = await root.GetFolderAsync("Templates");
            var list = new List<Models.Group>();
            foreach (var subfolder in await folder.GetFoldersAsync())
            {
                var files = await subfolder.GetFilesAsync();
                var group = new Models.Group
                {
                    Title = subfolder.Name,
                    Items = files.Select(x => new Models.Item { Title = x.DisplayName, Path = x.Path }),
                };
                list.Add(group);
            }

            SessionState[nameof(JustXaml)] = list;
            NavigationService.Navigate(typeof(Views.MainPage), nameof(JustXaml));
            await Task.CompletedTask;
        }
    }
}

