using System;
using Template10.Common;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace JustKiosk.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Navigate();
        }

        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            await Navigate();
        }

        private async System.Threading.Tasks.Task Navigate()
        {
            Uri uri;
            if (Uri.TryCreate(ViewModel.HomeUrl, UriKind.Absolute, out uri))
                MyWebView.Navigate(uri);
            else
            {
                uri = new Uri("ms-appx:///Assets/WebView/default.html");
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var html = await FileIO.ReadTextAsync(file);
                MyWebView.NavigateToString(html);
            }
        }
    }
}
