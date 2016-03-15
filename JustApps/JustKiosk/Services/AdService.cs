using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustKiosk.Services
{
    public class AdService
    {
        SettingsService _SettingsService;
        public AdService(SettingsService settingsService = null)
        {
            _SettingsService = settingsService ?? new SettingsService();
        }

        private void OnLicenseInformationChanged()
        {
            throw new NotImplementedException();
        }

        public event Template10.Common.TypedEventHandler<Results> Completed;

        async Task<string> PostalCodeAsync()
        {
            try
            {
                var geoLocator = new Windows.Devices.Geolocation.Geolocator();
                var geoPosition = await geoLocator.GetGeopositionAsync();
                return geoPosition.CivicAddress?.PostalCode;
            }
            catch
            {
                return null;
            }
        }

        public enum Results { Completed, Cancelled, Exception }

        public async void ShowVideoAd()
        {
            var postalCode = await PostalCodeAsync();
            var videoAd = new InterstitialAd();
            if (postalCode != null)
                videoAd.PostalCode = postalCode;

            videoAd.Completed += (s, e) => Completed?.Invoke(this, Results.Completed);
            videoAd.Cancelled += (s, e) => Completed?.Invoke(this, Results.Cancelled);
            videoAd.ErrorOccurred += (s, e) =>
            {
                Debugger.Break();
                Completed?.Invoke(this, Results.Exception);
            };
            videoAd.AdReady += (s, e) => videoAd.Show();

            var appId = _SettingsService.VideoAdAppId;
            var unitId = _SettingsService.VideoAdUnitId;
            videoAd.RequestAd(AdType.Video, appId, unitId);
        }
    }
}
