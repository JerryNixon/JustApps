using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Microsoft.HockeyApp;

namespace JustTrek
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public App()
        {
            InitializeComponent();

            var settings = new Services.SettingsService();
            var appid = settings.hockeyapp_appid;
            HockeyClient.Current.Configure(appid, new TelemetryConfiguration
            {
                EnableDiagnostics = true
            });
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e) => new Views.FeedPage();

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }
    }
}

