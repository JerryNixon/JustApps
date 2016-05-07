using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace JustKiosk.Services
{
    public class SettingsService
    {
        public static SettingsService Instance = new SettingsService();

        Template10.Services.SettingsService.SettingsHelper _SettingsHelper;
        FileHelper _FileHelper;

        private SettingsService()
        {
            _SettingsHelper = new Template10.Services.SettingsService.SettingsHelper();
            _FileHelper = new FileHelper();
        }

        public async Task<List<string>> LoadBlackListAsync()
        {
            var list = await _FileHelper.ReadFileAsync<List<string>>("BlackList", StorageStrategies.Roaming) ?? new List<string>();
            if (!list.Any())
            {
                list.Add("google.com");
                list.Add("bing.com");
                list.Add("yahoo.com");
                list.Add("aol.com");
                list.Add("outlook.com");
                list.Add("gmail.com");
            }
            return list;
        }
        public async Task SaveBlackListAsync(List<string> value) => await _FileHelper.WriteFileAsync("BlackList", value, StorageStrategies.Roaming);

        public bool IntroShown
        {
            get { return _SettingsHelper.Read(nameof(IntroShown), false, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(IntroShown), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public string HomeUrl
        {
            get { return _SettingsHelper.Read(nameof(HomeUrl), string.Empty, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(HomeUrl), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public int RefreshMinutes
        {
            get { return _SettingsHelper.Read(nameof(RefreshMinutes), 5, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(RefreshMinutes), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public int AdminPassword
        {
            get { return _SettingsHelper.Read(nameof(AdminPassword), 1234, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(AdminPassword), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public bool ShowNavButtons
        {
            get { return _SettingsHelper.Read(nameof(ShowNavButtons), false, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(ShowNavButtons), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public bool PreventWhenFace
        {
            get { return _SettingsHelper.Read(nameof(PreventWhenFace), false, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(PreventWhenFace), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public bool IsWebContent
        {
            get { return _SettingsHelper.Read(nameof(IsWebContent), true, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(IsWebContent), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public string CameraSubFolder = "Just4Kiosks";
        public string HockeyAppId = "8f97329ad3c74a26ba2801d7c9f578ec";
        public TimeSpan VideoAdTimeSpan = TimeSpan.FromMinutes(30);

#if DEBUG
        public string VideoAdAppId = "d25517cb-12d4-4699-8bdc-52040c712cab";
        public string VideoAdUnitId = "11389925";
#else
        public string VideoAdAppId = "e77284e2-160f-4c7a-94bc-c2ea8f9320f7";
        public string VideoAdUnitId = "11593236";
#endif
    }
}
