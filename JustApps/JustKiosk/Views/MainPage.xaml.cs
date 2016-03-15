using System;
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
            ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX | Windows.UI.Xaml.Input.ManipulationModes.TranslateY;
            Loaded += MainPage_Loaded;
        }

        //PathGeometry _PathGeometry;

        //private void MainPage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        //{
        //    _PathGeometry = new PathGeometry { FillRule = FillRule.Nonzero };
        //    var figure = new PathFigure { IsFilled = true, IsClosed = true, StartPoint = new Point(0, 0) };
        //    figure.Segments.Add(new LineSegment { Point = new Point() });
        //    _PathGeometry.Figures.Add(figure);
        //}

        //private void MainPage_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        //{
        //    if (!IsDrawing) return;
        //}

        //private void MainPage_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsDrawing { get { return (bool)GetValue(IsDrawingProperty); } set { SetValue(IsDrawingProperty, value); } }
        //public static readonly DependencyProperty IsDrawingProperty =
        //    DependencyProperty.Register(nameof(IsDrawing), typeof(bool), typeof(MainPage), new PropertyMetadata(false, IsDrawingChanged));
        //private static void IsDrawingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var page = d as MainPage;
        //    if ((bool)e.NewValue)
        //    {
        //        page.ManipulationDelta += page.MainPage_ManipulationDelta;
        //        page.ManipulationStarted += page.MainPage_ManipulationStarted;
        //        page.ManipulationCompleted += page.MainPage_ManipulationCompleted;
        //    }
        //    else
        //    {
        //        page.ManipulationDelta -= page.MainPage_ManipulationDelta;
        //        page.ManipulationStarted -= page.MainPage_ManipulationStarted;
        //        page.ManipulationCompleted -= page.MainPage_ManipulationCompleted;
        //    }
        //}

        private void MainPage_Loaded(object sender, RoutedEventArgs args)
        {
            _AdService = new Services.AdService();
            _SettingsService = new Services.SettingsService();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            if (!_SettingsService.IntroShown)
            {
                _SettingsService.IntroShown = true;
                VisualStateManager.GoToState(this, IntroState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, NormalState.Name, true);
            }

            // ads are only in trial mode
            bool IsTrial = false;
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
        private async void Home_Click(object sender, RoutedEventArgs e) => await NavigateAsync();
        private async void Navigate_Click(object sender, RoutedEventArgs e) => await NavigateAsync();
        private async System.Threading.Tasks.Task NavigateAsync()
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

        private void ShowHelp_Click(object sender, RoutedEventArgs e) => VisualStateManager.GoToState(this, HelpState.Name, true);
        private async void Contact_Click(object sender, RoutedEventArgs e) =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:support@liquid47.com?subject=Just for Kiosks&body=My feedback is..."));
    }
}
