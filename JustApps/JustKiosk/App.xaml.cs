using Microsoft.HockeyApp;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace JustKiosk
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();

            var settings = Services.SettingsService.Instance;
            var appid = settings.HockeyAppId;
            HockeyClient.Current.Configure(appid, new TelemetryConfiguration
            { 
                EnableDiagnostics = true
            });
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }
    }
}

