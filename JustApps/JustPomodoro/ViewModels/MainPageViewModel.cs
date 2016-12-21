using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using System.Threading;
using Template10.Common;
using Windows.UI.Core;
using System.Diagnostics;
using Windows.UI.Popups;
using Template10.Utils;
using System.Collections.ObjectModel;

namespace JustPomodoro.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        WunderlistSdk.Models.Task _SelectedTask;
        public WunderlistSdk.Models.Task SelectedTask { get { return _SelectedTask; } set { Set(ref _SelectedTask, value); } }

        public ObservableCollection<Group<WunderlistSdk.Models.Task>> TaskLists { get; } = new ObservableCollection<Group<WunderlistSdk.Models.Task>>();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var repo = new WunderlistSdk.WunderlistRepository(App.WunderlistSettings);
            await repo.AuthenticateAsync();
            await repo.RefreshDatabaseAsync();
            var lists = await repo.Database.GetListsAsync();
            foreach (var list in lists)
            {
                var group = new Group<WunderlistSdk.Models.Task>
                {
                    Title = list.Title,
                    Items = await repo.Database.GetTasksAsync(list),
                };
            }
        }
    }

    public class Group<T>
    {
        public string Title { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}

