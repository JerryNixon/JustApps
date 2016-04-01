using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace JustKiosk.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        Services.SettingsService _SettingsService;
        DispatcherTimer timer = new DispatcherTimer();
        public AdminViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { }
            else
            {
                _SettingsService = Services.SettingsService.Instance;
                timer.Tick += (s, e) => Refresh?.Invoke(this, EventArgs.Empty);
                if (RefreshMinutes > 0)
                {
                    timer.Interval = TimeSpan.FromMinutes(RefreshMinutes);
                    timer.Start();
                }
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            BlackList = new ObservableCollection<string>(await _SettingsService.GetBlackListAsync());
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            await _SettingsService.SetBlackListAsync(BlackList.ToList());
        }

        public ObservableCollection<string> WhiteList { get; set; }
        public ObservableCollection<string> BlackList { get; set; }

        public string KioskId => Math.Abs(HardwareId().GetHashCode()).ToString();

        string HardwareId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);
            var bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);
            return BitConverter.ToString(bytes);
        }

        public event EventHandler Refresh;

        public string HomeUrl
        {
            get { return _SettingsService.HomeUrl; }
            set { _SettingsService.HomeUrl = value; }
        }

        public int RefreshMinutes
        {
            get { return _SettingsService.RefreshMinutes; }
            set
            {
                _SettingsService.RefreshMinutes = value; RaisePropertyChanged(nameof(RefreshMinutes));
                timer.Stop();
                if (value > 0)
                {
                    timer.Interval = TimeSpan.FromMinutes(RefreshMinutes);
                    timer.Start();
                }
            }
        }

        public int ActualPin
        {
            get { return _SettingsService.AdminPassword; }
            set { _SettingsService.AdminPassword = value; }
        }

        public bool ShowNavButtons
        {
            get { return _SettingsService.ShowNavButtons; }
            set { _SettingsService.ShowNavButtons = value; RaisePropertyChanged(nameof(ShowNavButtons)); }
        }

        public bool PreventWhenFace
        {
            get { return _SettingsService.PreventWhenFace; }
            set { _SettingsService.PreventWhenFace = value; RaisePropertyChanged(nameof(PreventWhenFace)); }
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
            RaisePropertyChanged(nameof(ActualPin));
        }
    }
}

