using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace JustKiosk.Views
{
    public sealed partial class MainPage : Page
    {
        DispatcherTimer _timer;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs args)
        {
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ViewModel.AdminViewModel.Refresh += async (s, e) => await NavigateAsync();

            var host = Windows.ApplicationModel.LockScreen.LockApplicationHost.GetForCurrentView();
            var helpUrl = "https://msdn.microsoft.com/en-us/library/windows/hardware/mt620040(v=vs.85).aspx";
            if (host == null)
            {
                await NavigateAsync(helpUrl);
            }
            else
            {
                host.Unlocking += (s, e) => App.Current.Exit();
            }

            var settingsService = Services.SettingsService.Instance;
            if (settingsService.IntroShown)
            {
                VisualStateManager.GoToState(this, NoneState.Name, true);
            }
            else
            {
                settingsService.IntroShown = true;
                VisualStateManager.GoToState(this, IntroState.Name, true);
            }

            Controls.Settings.Navigate += async (s, e) => await NavigateAsync();
            Controls.Settings.ShowHelp += async (s, e) =>
            {
                VisualStateManager.GoToState(this, HelpState.Name, true);
                await NavigateAsync(helpUrl);
            };

            // ads are only in trial mode
            bool IsTrial = true;
            try { IsTrial = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.IsTrial; }
            catch { }
            if (!IsTrial)
            {
                var adService = Services.AdService.Instance;
                _timer = new DispatcherTimer { Interval = settingsService.VideoAdTimeSpan };
                _timer.Tick += (s, e) => adService.ShowVideoAd();
                _timer.Start();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) => await NavigateAsync();

        private async System.Threading.Tasks.Task NavigateAsync(string url = null)
        {
            Uri uri;
            url = url ?? ViewModel.AdminViewModel.HomeUrl;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                MyWebView.Navigate(uri);
            else
            {
                uri = new Uri("ms-appx:///Assets/WebView/default.html");
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var html = await FileIO.ReadTextAsync(file);
                MyWebView.NavigateToString(html);
            }
        }

        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MyWebView.CanGoBack) MyWebView.GoBack();
        }
    }
}
