using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.LockScreen;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace JustKiosk.Views
{
    public sealed partial class MainPage : Page
    {
        DispatcherTimer _timer;
        string _HelpUrl = "https://msdn.microsoft.com/en-us/library/windows/hardware/mt620040(v=vs.85).aspx";
        Services.SettingsService _SettingsService;
        Services.CameraService.CameraService _CameraService;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            _SettingsService = Services.SettingsService.Instance;
            _CameraService = Services.CameraService.CameraService.Instance;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs args)
        {
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            SetupAutoRefresh();

            SetupBlackListFilter();

            SetupAdvertisement();

            SetupAssignedAccess();

            ShowOneTimeIntro();

            HandleAdminGoButton();

            HandleAdminHelpButton();
        }

        private void SetupAdvertisement()
        {
            if (ViewModel.AdminViewModel.IsTrial)
            {
                var adService = Services.AdService.Instance;
                _timer = new DispatcherTimer
                {
                    Interval = _SettingsService.VideoAdTimeSpan
                };
                _timer.Tick += (s, e) =>
                {
                    if (ViewModel.AdminViewModel.IsTrial)
                    {
                        adService.ShowVideoAd();
                    }
                    else
                    {
                        _timer.Stop();
                    }
                };
                _timer.Start();
            }
        }

        private void HandleAdminHelpButton()
        {
            Controls.Settings.ShowHelp += async (s, e) =>
            {
                VisualStateManager.GoToState(this, HelpState.Name, true);
                await NavigateAsync(_HelpUrl);
            };
        }

        private void HandleAdminGoButton()
        {
            Controls.Settings.Navigate += async (s, e) =>
            {
                await NavigateAsync();
            };
        }

        private void ShowOneTimeIntro()
        {
            if (_SettingsService.IntroShown)
            {
                VisualStateManager.GoToState(this, NoneState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, IntroState.Name, true);
                _SettingsService.IntroShown = true;
            }
        }

        private async void SetupAssignedAccess()
        {
            var host = LockApplicationHost.GetForCurrentView();
            var assignedAccess = (host != null);
            if (assignedAccess)
            {
                host.Unlocking += (s, e) => App.Current.Exit();
            }
            else
            {
                await NavigateAsync(_HelpUrl);
            }
        }

        private void SetupBlackListFilter()
        {
            MyWebView.NavigationStarting += async (s, e) =>
            {
                if (e.Uri == null)
                {
                    return;
                }
                if (await FoundInBlacklistAsync(e.Uri))
                {
                    var home = new Uri(_SettingsService.HomeUrl);
                    if (home != MyWebView.Source)
                    {
                        if (!await FoundInBlacklistAsync(home))
                        {
                            await NavigateAsync();
                        }
                    }
                }
                var uri = e.Uri?.ToString();
                if (uri == null)
                {
                    return;
                }
                var list = await _SettingsService.LoadBlackListAsync();
                foreach (var item in list)
                {
                    if (uri?.ToLower().Contains(item.ToLower()) ?? false)
                    {
                        var home = _SettingsService.HomeUrl;
                        if (new Uri(home) != MyWebView.Source)
                        {
                            await NavigateAsync();
                        }
                        return;
                    }
                }
            };
        }

        DateTime startDateTime = DateTime.Now;
        DateTime targetDateTime = DateTime.Now;
        DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(.1) };
        private void SetupAutoRefresh()
        {
            startDateTime = DateTime.Now;
            targetDateTime = DateTime.Now.Add(ViewModel.AdminViewModel.RefreshTimeSpan);
            ViewModel.AdminViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.AdminViewModel.RefreshMinutes))
                {
                    timer.Stop();
                    startDateTime = DateTime.Now;
                    targetDateTime = DateTime.Now.Add(ViewModel.AdminViewModel.RefreshTimeSpan);
                    timer.Start();
                }
            };
            timer.Tick += async (s, e) =>
            {
                timer.Stop();
                try
                {
                    var total = targetDateTime.TimeOfDay.Subtract(startDateTime.TimeOfDay).TotalMinutes;
                    if (total == 0)
                    {
                        return;
                    }
                    var remain = targetDateTime.TimeOfDay.Subtract(DateTime.Now.TimeOfDay).TotalMinutes;
                    var percent = remain / total;
                    var angle = 360 - (360 * percent);
                    // Debug.WriteLine($"{remain}:{total} {percent}% {angle}deg");
                    TimerRing.StartAngle = angle;
                    if (angle < 358)
                    {
                        return;
                    }
                    var okayToGoHome = !(_SettingsService.PreventWhenFace && (Services.CameraService.CameraService.FacesCount ?? 0) > 0);
                    if (okayToGoHome)
                    {
                        startDateTime = DateTime.Now;
                        targetDateTime = DateTime.Now.Add(ViewModel.AdminViewModel.RefreshTimeSpan);
                        await NavigateAsync();
                    }
                    else
                    {
                        startDateTime = DateTime.Now;
                        targetDateTime = DateTime.Now.Add(TimeSpan.FromMinutes(.5));
                    }
                }
                finally
                {
                    timer.Start();
                }
            };
            // visual queue
            _CameraService.FaceDetected += (s, e) =>
            {
                if (e.ResultFrame.DetectedFaces.Count == 0)
                {
                    TimerRing.Fill = new SolidColorBrush(Colors.Silver);
                }
                else
                {
                    TimerRing.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                }
            };
            timer.Start();
        }

        private async Task<bool> FoundInBlacklistAsync(Uri uri)
        {
            var list = await _SettingsService.LoadBlackListAsync();
            var link = uri.ToString().ToLower();
            return list
                .Select(x => x.ToLower())
                .Any(x => link.Contains(x));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) => await NavigateAsync();

        private async Task NavigateAsync(string url = null)
        {
            Uri uri;
            url = url ?? ViewModel.AdminViewModel.HomeUrl;
            var navigateToDefaultPage = new Func<Task>(async () =>
            {
                uri = new Uri("ms-appx:///Assets/WebView/default.html");
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var html = await FileIO.ReadTextAsync(file);
                MyWebView.NavigateToString(html);
            });
            if (!ViewModel.AdminViewModel.IsWebContent)
            {
                var folder = ViewModel.AdminViewModel.LocalFolder;
                if (folder == null)
                {
                    folder = await ViewModel.AdminViewModel.RememberLocalFolderAsync();
                }
                if (folder == null)
                {
                    Debugger.Break();
                    await navigateToDefaultPage();
                }
                else
                {
                    var resolver = new Services.LocalResolver(folder);
                    var local = MyWebView.BuildLocalStreamUri("local", "default.html");
                    MyWebView.NavigateToLocalStreamUri(local, resolver);
                }
            }
            else if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                MyWebView.Navigate(uri);
            }
            else
            {
                await navigateToDefaultPage();
            }
        }

        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync();
            startDateTime = DateTime.Now;
            targetDateTime = DateTime.Now.Add(ViewModel.AdminViewModel.RefreshTimeSpan);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MyWebView.CanGoBack) MyWebView.GoBack();
        }
    }
}
