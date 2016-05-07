using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            var list = await _SettingsService.LoadBlackListAsync();
            BlackListEditor.ItemsSource = new ObservableCollection<string>(list);
            BlackListEditor.ListChanged += async (s, args) =>
            {
                await _SettingsService.SaveBlackListAsync(BlackListEditor.ItemsSource.ToList());
            };
            UpdateVisualStates();
        }

        public static event EventHandler Navigate;
        public static event EventHandler ShowHelp;
        private void RaiseNavigate() => Navigate?.Invoke(this, EventArgs.Empty);
        private void RaiseShowHelp() => ShowHelp?.Invoke(this, EventArgs.Empty);
        public void SetPin() => ViewModel.SetPin();
        private async void PickFolder()
        {
            var folder = await ViewModel.PickFolderAsync();
            UpdateVisualStates();
        }

        private async void SendSupportEmail()
        {
            var to = Uri.EscapeUriString("support@liquid47.com");
            var subject = Uri.EscapeUriString("Just for Kiosks");
            var body = Uri.EscapeUriString("My feedback is...");
            var uri = new Uri($"mailto:{to}?subject={subject}&body={body}");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        void UpdateVisualStates()
        {
            if (ViewModel.IsWebContent)
            {
                VisualStateManager.GoToState(this, WebContentState.Name, true);
                if (!ViewModel.HomeUrl.StartsWith("http"))
                {
                    ViewModel.HomeUrl = string.Empty;
                }
            }
            else
            {
                VisualStateManager.GoToState(this, LocalContentState.Name, true);
                if (ViewModel.LocalFolder == null)
                {
                    ViewModel.HomeUrl = "Invalid";
                }
                else
                {
                    ViewModel.HomeUrl = $"default.html";
                }
            }
        }

        ViewModels.AdminViewModel ViewModel => DataContext as ViewModels.AdminViewModel;

        private async void IsTrial_Toggled(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && (sender as ToggleSwitch).IsOn)
            {
                await ViewModel.RequestAppPurchaseAsync();
                (sender as ToggleSwitch).IsOn = !ViewModel.IsTrial;
            }
        }
    }
}
