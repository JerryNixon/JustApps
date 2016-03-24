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
        Services.SettingsService _SettingsService;
        Services.AdService _AdService;
        DispatcherTimer _timer;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += MainPage_Loaded;

            var lockHost = Windows.ApplicationModel.LockScreen.LockApplicationHost.GetForCurrentView();
            if (lockHost != null)
            {
                lockHost.Unlocking += (s, e) => App.Current.Exit();
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs args)
        {
            _AdService = new Services.AdService();
            _SettingsService = new Services.SettingsService();

            if (!_SettingsService.IntroShown)
            {
                _SettingsService.IntroShown = true;
                VisualStateManager.GoToState(this, IntroState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, NormalState.Name, true);
            }
            Controls.Settings.Navigate += async (s, e) => await NavigateAsync();
            Controls.Settings.ShowHelp += (s, e) => VisualStateManager.GoToState(this, HelpState.Name, true);

            // ads are only in trial mode
            bool IsTrial = true;
            try { IsTrial = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.IsTrial; }
            catch { }
            if (!IsTrial)
            {
                _timer = new DispatcherTimer { Interval = _SettingsService.VideoAdTimeSpan };
                _timer.Tick += (s, e) => _AdService.ShowVideoAd();
                _timer.Start();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) => await NavigateAsync();

        private async System.Threading.Tasks.Task NavigateAsync()
        {
            Uri uri;
            string homeUrl = ViewModel.AdminViewModel.HomeUrl;
            if (Uri.TryCreate(homeUrl, UriKind.Absolute, out uri))
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
    }
}
