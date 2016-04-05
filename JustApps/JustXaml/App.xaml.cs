using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.ApplicationModel.Activation;

namespace JustXaml
{
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App() { InitializeComponent(); }

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

