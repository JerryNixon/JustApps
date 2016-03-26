using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace JustKiosk.Controls
{
    public sealed partial class Settings : UserControl
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        public static event EventHandler Navigate;
        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            Navigate?.Invoke(this, EventArgs.Empty);
        }

        public static event EventHandler ShowHelp;
        private void ShowHelp_Click(object sender, RoutedEventArgs e)
        {
            ShowHelp?.Invoke(this, EventArgs.Empty);
        }

        private async void Contact_Click(object sender, RoutedEventArgs e)
        {
            var to = "support@liquid47.com";
            var subject = "Just for Kiosks";
            var body = "My feedback is...";
            var uri = new Uri($"mailto:{to}?subject={subject}&body={body}");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        public void SetPin()
        {
            (DataContext as ViewModels.AdminViewModel).SetPin();
        }

        private async void MyCaptureElement_Loaded(object sender, RoutedEventArgs args)
        {
            var settings = Services.SettingsService.Instance;
            var service = new Services.CameraService(sender as CaptureElement, settings.CameraSubFolder);
            await service.InitializeAsync();
            await service.StartPreviewAsync();
            MyCaptureGrid.Children.Add(service.FacesCanvas);
            service.FaceDetected += (s, e) => { };
        }
    }
}
