using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JustTrek.Models;
using Template10.Utils;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JustTrek.Views
{
    public sealed partial class FeedPage : Page
    {
        public FeedPage()
        {
            InitializeComponent();
        }

        private async void Item_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var r = sender as RelativePanel;
            var i = r.DataContext as Models.Item;
            await Launcher.LaunchUriAsync(i.Link);
        }
    }
}
