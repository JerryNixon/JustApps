using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JustTrek.Models;
using Template10.Utils;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace JustTrek.Views
{
    public sealed partial class RssPage : Page
    {
        public RssPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var json = Template10.Services.SerializationService.SerializationService.Json;
            var parameter = json.Deserialize<Parameter>(e.Parameter?.ToString());
            var service = new Services.DataService();
            Title = parameter.Title;
            var facebook = await service.GetRssItemsAsync(new Uri(parameter.Param));
            if (facebook != null)
            {
                Items.AddRange(facebook.Select(x => new Item(x)), true);
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
    }
}
