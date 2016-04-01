using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
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
        Services.SettingsService _SettingsService;

        public Settings()
        {
            this.InitializeComponent();
            _SettingsService = Services.SettingsService.Instance;
            Loaded += Settings_Loaded;
        }

        private async void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            var list = await _SettingsService.GetBlackListAsync();
            var observable = new ObservableCollection<string>(list);
            BlackListEditor.ItemsSource = observable;
            BlackListEditor.ListChanged += async (s, args) => 
            {
                await _SettingsService.SetBlackListAsync(BlackListEditor.ItemsSource.ToList());
            };
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


    }
}
