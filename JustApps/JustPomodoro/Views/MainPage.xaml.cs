using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using JustPomodoro.Services;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.UI.Popups;
using JustPomodoro.Services.Json;
using SQLite;

namespace JustPomodoro.Views
{
    [Table]
    public class Settings
    {

    }

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            var helper = new WunderlistHelper(App.WunderlistSettings);
            var authenticated = await helper.AuthenticateAsync();
            if (!authenticated)
            {
                await new MessageDialog("Not authenticated").ShowAsync();
                return;
            }
            var groups = new List<object>();
            var folders = await helper.GetFoldersAsync();
            var folderGroups = new List<Group<Folder, Group<List, Task>>>();
            foreach (var folder in folders)
            {
                var folderGroup = new Group<Folder, Group<List, Task>>
                {
                    Parent = folder,
                    Title = folder.title
                };
                folderGroups.Add(folderGroup);
                var lists = await helper.GetListsAsync(folder);
                var folderLists = new List<Group<List, Task>>();
                folderGroup.Children = folderLists;
                foreach (var list in lists)
                {
                    var listGroup = new Group<List, Task>
                    {
                        Parent = list,
                        Title = list.title,
                        Children = await helper.GetTasksAsync(list)
                    };
                    folderLists.Add(listGroup);
                }
            }
        }
    }

    public class Group<P, C>
    {
        public string Title { get; set; }
        public P Parent { get; set; }
        public IEnumerable<C> Children { get; set; }
    }
}
