using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace JustKiosk.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        Services.SettingsService _SettingsService;
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { }
            else
            {
                _SettingsService = new Services.SettingsService();
            }
        }

        public string HomeUrl
        {
            get { return _SettingsService.HomeUrl; }
            set { _SettingsService.HomeUrl = value; }
        }

        string _AdminPin = string.Empty;
        public string AdminPin
        {
            get { return _AdminPin; }
            set { Set(ref _AdminPin, value); }
        }

        bool _IsAdmin = default(bool);
        public bool IsAdmin
        {
            get { return _IsAdmin; }
            set { Set(ref _IsAdmin, value); }
        }

        public void Authenticate()
        {
            IsAdmin = (AdminPin == _SettingsService.AdminPassword.ToString());
            if (!IsAdmin)
                IsAdmin = (AdminPin == new string(DateTime.Now.ToString("ddyy").Reverse().ToArray()));
            AdminPin = string.Empty;
        }

        public async void SetPin()
        {
            if (string.IsNullOrEmpty(AdminPin))
            {
                await new Windows.UI.Xaml.Controls.ContentDialog
                {
                    Title = "Error",
                    Content = $"Your new pin cannot be blank.",
                    PrimaryButtonText = "Ok"
                }.ShowAsync();
            }
            else
            {
                _SettingsService.AdminPassword = int.Parse(AdminPin);
                await new Windows.UI.Xaml.Controls.ContentDialog
                {
                    Title = "New pin",
                    Content = $"Don't forget. Your new pin is {AdminPin}.",
                    PrimaryButtonText = "Ok"
                }.ShowAsync();
                AdminPin = string.Empty;
            }
        }
    }
}

