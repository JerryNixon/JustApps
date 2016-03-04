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

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await Task.CompletedTask;
        }

        public string HomeUrl { get { return _SettingsService.HomeUrl; } set { _SettingsService.HomeUrl = value; } }

        string _AdminPassword = string.Empty;
        public string AdminPassword { get { return _AdminPassword; } set { Set(ref _AdminPassword, value); } }

        bool _IsAdmin = default(bool);
        public bool IsAdmin { get { return _IsAdmin; } set { Set(ref _IsAdmin, value); } }

        public void Add1() { if (AdminPassword.Length < 4) AdminPassword += "1"; }
        public void Add2() { if (AdminPassword.Length < 4) AdminPassword += "2"; }
        public void Add3() { if (AdminPassword.Length < 4) AdminPassword += "3"; }
        public void Add4() { if (AdminPassword.Length < 4) AdminPassword += "4"; }
        public void Add5() { if (AdminPassword.Length < 4) AdminPassword += "5"; }
        public void Add6() { if (AdminPassword.Length < 4) AdminPassword += "6"; }
        public void Add7() { if (AdminPassword.Length < 4) AdminPassword += "7"; }
        public void Add8() { if (AdminPassword.Length < 4) AdminPassword += "8"; }
        public void Add9() { if (AdminPassword.Length < 4) AdminPassword += "9"; }
        public void Add0() { if (AdminPassword.Length < 4) AdminPassword += "0"; }
        public void AddUndo() { AdminPassword = string.Empty; }

        public void Authenticate()
        {
            IsAdmin = (AdminPassword == _SettingsService.AdminPassword.ToString());
            AdminPassword = string.Empty;
        }
    }
}

