using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace JustD3
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();
            RequestedTheme = Windows.UI.Xaml.ApplicationTheme.Light;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            ModalDialog.ModalContent = new TextBlock
            {
                Text = "Loading...",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            await NavigationService.NavigateAsync(typeof(Views.MainPage));
        }
    }
}

