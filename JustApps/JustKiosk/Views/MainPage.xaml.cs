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
            //Loaded += (s, e) =>
            //{
            //    PathData = new PathGeometry { FillRule = FillRule.Nonzero };
            //    var pathData = _SettingsService.PathData;
            //    if (pathData != null)
            //    {
            //        PathData.Figures.Add(WindowBounds);
            //        PathData.Figures.Add(pathData);
            //    }
            //};
        }

        //PathFigure WindowBounds
        //{
        //    get
        //    {
        //        var bounds = Window.Current.Bounds;
        //        var figure = new PathFigure { IsFilled = true, IsClosed = true, StartPoint = new Point(0, 0) };
        //        figure.Segments.Add(new LineSegment { Point = new Point(bounds.Width, 0) });
        //        figure.Segments.Add(new LineSegment { Point = new Point(bounds.Width, bounds.Height) });
        //        figure.Segments.Add(new LineSegment { Point = new Point(0, bounds.Height) });
        //        return figure;
        //    }
        //}

        //public PathGeometry PathData { get { return (PathGeometry)GetValue(PathDataProperty); } set { SetValue(PathDataProperty, value); } }
        //public static readonly DependencyProperty PathDataProperty =
        //    DependencyProperty.Register(nameof(PathData), typeof(PathGeometry), typeof(MainPage), new PropertyMetadata(null));

        //private void MainPage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        //{
        //    if (IsDrawing)
        //    {
        //        PathData.Figures[1].IsFilled = true;
        //        PathData.Figures[1].IsClosed = true;
        //        PathData.Figures[1].Segments.Clear();
        //        PathData.Figures[1].StartPoint = e.Position;
        //    }
        //}

        //private void MainPage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        //{
        //    if (IsDrawing)
        //    {
        //        PathData.Figures[1].Segments.Add(new LineSegment { Point = e.Position });
        //    }
        //}

        //private void MainPage_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        //{
        //    if (IsDrawing)
        //    {
        //        _SettingsService.PathData = PathData.Figures[1];
        //    }
        //}

        private void EditPath_Click(object sender, RoutedEventArgs e)
        {
            // IsDrawing = ViewModel.IsAdmin = true;
            VisualStateManager.GoToState(this, BoundaryState.Name, true);
        }

        //public bool IsDrawing { get { return (bool)GetValue(IsDrawingProperty); } set { SetValue(IsDrawingProperty, value); } }
        //public static readonly DependencyProperty IsDrawingProperty =
        //    DependencyProperty.Register(nameof(IsDrawing), typeof(bool), typeof(MainPage), new PropertyMetadata(false, IsDrawingChanged));
        //private static void IsDrawingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var page = d as MainPage;
        //    if ((bool)e.NewValue)
        //    {
        //        page.BoundaryPath.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        //        page.BoundaryPath.ManipulationDelta += page.MainPage_ManipulationDelta;
        //        page.BoundaryPath.ManipulationStarted += page.MainPage_ManipulationStarted;
        //        page.BoundaryPath.ManipulationCompleted += page.MainPage_ManipulationCompleted;

        //        page.PathData.Figures.Clear();
        //        page.PathData.Figures.Add(page.WindowBounds);
        //        var figure = new PathFigure { IsFilled = true, IsClosed = true };
        //        page.PathData.Figures.Add(figure);

        //    }
        //    else
        //    {
        //        page.BoundaryPath.ManipulationMode = ManipulationModes.None;
        //        page.BoundaryPath.ManipulationDelta -= page.MainPage_ManipulationDelta;
        //        page.BoundaryPath.ManipulationStarted -= page.MainPage_ManipulationStarted;
        //        page.BoundaryPath.ManipulationCompleted -= page.MainPage_ManipulationCompleted;
        //    }
        //}

        private void MainPage_Loaded(object sender, RoutedEventArgs args)
        {
            _AdService = new Services.AdService();
            _SettingsService = new Services.SettingsService();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            ViewModel.IsAdmin = true;
            VisualStateManager.GoToState(this, AdminState.Name, true);
            return;

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
