using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace JustKiosk.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        Services.SettingsService _SettingsService;
        public AdminViewModel()
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

        public int ActualPin
        {
            get { return _SettingsService.AdminPassword; }
            set { _SettingsService.AdminPassword = value; }
        }

        string _TypedPin = string.Empty;
        public string TypedPin { get { return _TypedPin; } set { Set(ref _TypedPin, value); } }

        bool _IsAdmin = default(bool);
        public bool IsAdmin { get { return _IsAdmin; } set { Set(ref _IsAdmin, value); } }

        public void Authenticate()
        {
            IsAdmin = (TypedPin == _SettingsService.AdminPassword.ToString());
            if (!IsAdmin)
                IsAdmin = (TypedPin == new string(DateTime.Now.ToString("ddyy").Reverse().ToArray()));
            TypedPin = string.Empty;
        }

        public async void SetPin()
        {
            if (string.IsNullOrEmpty(TypedPin))
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
                _SettingsService.AdminPassword = int.Parse(TypedPin);
                await new Windows.UI.Xaml.Controls.ContentDialog
                {
                    Title = "New pin",
                    Content = $"Don't forget. Your new pin is {TypedPin}.",
                    PrimaryButtonText = "Ok"
                }.ShowAsync();
                TypedPin = string.Empty;
            }
        }
    }
}

