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

        public string HomeUrl
        {
            get { return _helper.Read(nameof(HomeUrl), string.Empty); }
            set { _helper.Write(nameof(HomeUrl), value); }
        }

        public int AdminPassword
        {
            get { return _helper.Read(nameof(AdminPassword), 1234); }
            set { _helper.Write(nameof(AdminPassword), value); }
        }

        public string HockeyAppId = "8f97329ad3c74a26ba2801d7c9f578ec";
        public string HockeyAppSecret = "62d21b5846cd1c3982828dd4ebbc6ee1";
    }
}
