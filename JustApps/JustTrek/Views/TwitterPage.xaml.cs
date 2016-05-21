using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AppStudio.DataProviders.Twitter;
using JustTrek.Models;
using Template10.Mvvm;
using Template10.Utils;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace JustTrek.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TwitterPage : Page
    {
        public TwitterPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var json = Template10.Services.SerializationService.SerializationService.Json;
            var parameter = json.Deserialize<Parameter>(e.Parameter?.ToString());
            var service = new Services.DataService();
            Title = parameter.Title;
            var twitter = await service.GetTwitterItemsAsync(parameter.Param, TwitterQueryType.User);
            if (twitter != null)
            {
                Items.AddRange(twitter.Select(x => new Item(x)), true);
            }
        }

        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string),
                typeof(FacebookPage), new PropertyMetadata("Title"));

        DelegateCommand<Item> _ItemClickCommand;
        public DelegateCommand<Item> ItemClickCommand
           => _ItemClickCommand ?? (_ItemClickCommand = new DelegateCommand<Item>(async (o) =>
           {
               await Launcher.LaunchUriAsync(o.Link);
           }, (o) => true));
    }
}
