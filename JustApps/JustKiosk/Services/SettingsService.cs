using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustKiosk.Services
{
    public class SettingsService
    {
        Template10.Services.SettingsService.SettingsHelper _helper;
        public SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool IntroShown
        {
            get { return _helper.Read(nameof(IntroShown), false, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _helper.Write(nameof(IntroShown), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public string HomeUrl
        {
            get { return _helper.Read(nameof(HomeUrl), "http://bing.com", Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _helper.Write(nameof(HomeUrl), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
        }

        public int AdminPassword
        {
            get { return _helper.Read(nameof(AdminPassword), 1234, Template10.Services.SettingsService.SettingsStrategies.Roam); }
            set { _helper.Write(nameof(AdminPassword), value, Template10.Services.SettingsService.SettingsStrategies.Roam); }
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
