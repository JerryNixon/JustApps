using JustXaml.Services.FileHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace JustXaml.Services
{
    public class SettingsService
    {
        public static SettingsService Instance = new SettingsService();

        Template10.Services.SettingsService.SettingsHelper _SettingsHelper;
        Services.FileHelper.FileHelper _FileHelper;

        private SettingsService()
        {
            _SettingsHelper = new Template10.Services.SettingsService.SettingsHelper();
            _FileHelper = new Services.FileHelper.FileHelper();
        }

        public string HockeyAppId = "1793f927ab86444b99c173db1d6f92c9";
//#if DEBUG
//        public string VideoAdAppId = "d25517cb-12d4-4699-8bdc-52040c712cab";
//        public string VideoAdUnitId = "11389925";
//#else
//        public string VideoAdAppId = "e77284e2-160f-4c7a-94bc-c2ea8f9320f7";
//        public string VideoAdUnitId = "11593236";
//#endif
    }
}
