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
        Template10.Services.SettingsService.SettingsHelper _SettingsHelper;
        Services.FileHelper _FileHelper;
        public SettingsService()
        {
            _SettingsHelper = new Template10.Services.SettingsService.SettingsHelper();
            _FileHelper = new FileHelper();
        }

        public bool IntroShown
        {
            get { return _SettingsHelper.Read(nameof(IntroShown), false, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(IntroShown), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public string HomeUrl
        {
            get { return _SettingsHelper.Read(nameof(HomeUrl), "http://bing.com", Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(HomeUrl), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public int AdminPassword
        {
            get { return _SettingsHelper.Read(nameof(AdminPassword), 1234, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _SettingsHelper.Write(nameof(AdminPassword), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public PathFigure PathData
        {
            get { return _FileHelper.ReadFileAsync<PathFigure>(nameof(PathData), StorageStrategies.Roaming).Result; }
            set { _FileHelper.WriteFileAsync(nameof(PathData), value, StorageStrategies.Roaming); }
        }

        public string HockeyAppId = "8f97329ad3c74a26ba2801d7c9f578ec";
        public string HockeyAppSecret = "62d21b5846cd1c3982828dd4ebbc6ee1";

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
