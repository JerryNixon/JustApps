using System.Threading.Tasks;
using JustPomodoro.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Common;
using Windows.UI.Xaml.Data;
using WunderlistSdk;

namespace JustPomodoro
{
    [Bindable]
    sealed partial class App : BootStrapper
    {
        SettingsService _settingsService = SettingsService.Instance;

        public static WunderlistSettings WunderlistSettings = new WunderlistSettings
        {
            CLIENT_ID = "4d89e1f5d5a40b301d0" + "d",
            CLIENT_SECRET = "516ada762f40e42ab71329aae09db6e668925b0b1e75996195f1d5795ef" + "4",
            APP_URL = "http://jerrynixon.com",
            AUTH_CALLBACK_URL = "http://jerrynixon.com",
        };

        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
            RequestedTheme = _settingsService.AppTheme;
            CacheMaxDuration = _settingsService.CacheMaxDuration;
            ShowShellBackButton = _settingsService.UseShellBackButton;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            await NavigationService.NavigateAsync(typeof(Views.MainPage));
        }
    }
}

