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
            get { return _helper.Read(nameof(HomeUrl), "about:blank"); }
            set { _helper.Write(nameof(HomeUrl), value); }
        }

        public int AdminPassword
        {
            get { return _helper.Read(nameof(AdminPassword), 1234); }
            set { _helper.Write(nameof(AdminPassword), value); }
        }
    }
}
