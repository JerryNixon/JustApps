using JustXaml.Models;
using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using System.Diagnostics;

namespace JustXaml
{
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

            UnhandledException += (s, e) =>
            {
                Debugger.Break();
            };
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            if (DetermineStartCause(args) == AdditionalKinds.JumpListItem)
            {
                var e = args as FileActivatedEventArgs;
                if (e.Files.Any())
                {
                    if (startKind == StartKind.Launch)
                    {
                        SessionState[e.Files.First().Path] = e.Files.First();
                        NavigationService.Navigate(typeof(Views.MainPage), e.Files.First().Path);
                    }
                    else
                    {
                        var evt = Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>();
                        evt.Publish(new Models.File(e.Files.First() as StorageFile, File.Types.File));
                    };
                }
            }
            else
            {
                NavigationService.Navigate(typeof(Views.MainPage));
            }
            await Task.CompletedTask;
        }
    }
}

