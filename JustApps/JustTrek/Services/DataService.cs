using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AppStudio.DataProviders.Facebook;
using AppStudio.DataProviders.Twitter;
using Template10.Common;
using Windows.UI.Xaml.Controls;

namespace JustTrek.Services
{
    public partial class DataService
    {
        SettingsService _SettingSvc;
        public DataService()
        {
            _SettingSvc = new SettingsService();
        }

        public async Task<IEnumerable<FacebookSchema>> GetFaceBookItemsAsync(string query, int max = 20)
        {
            var items = await ExecuteAsync(async () =>
            {
                var config = new FacebookDataConfig
                {
                    UserId = query,
                };
                var auth = new FacebookOAuthTokens
                {
                    AppId = _SettingSvc.facebook_id,
                    AppSecret = _SettingSvc.facebook_secret,
                };
                var provider = new FacebookDataProvider(auth);
                return await provider.LoadDataAsync(config, max);
            });
            return items;
        }

        public async Task<IEnumerable<TwitterSchema>> GetTwitterItemsAsync(string query, TwitterQueryType type, int max = 20)
        {
            var items = await ExecuteAsync(async () =>
            {
                var config = new TwitterDataConfig
                {
                    Query = query,
                    QueryType = type,
                };
                var twitterDataProvider = new TwitterDataProvider(new TwitterOAuthTokens
                {
                    AccessToken = _SettingSvc.twitter_accessToken,
                    AccessTokenSecret = _SettingSvc.twitter_accessTokenSecret,
                    ConsumerKey = _SettingSvc.twitter_consumerKey,
                    ConsumerSecret = _SettingSvc.twitter_consumerSecret,
                });
                return await twitterDataProvider.LoadDataAsync(config, max);
            });
            return items;
        }

        async Task<T> ExecuteAsync<T>(Func<Task<T>> action, [CallerMemberName]string name = null)
        {
            Views.Busy.SetBusy(true, "Loading...");
            try
            {
                return await action();
            }
            catch (Exception e)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed {name}: {e.Message}",
                    PrimaryButtonText = "Okay",
                };
                await dialog.ShowAsync();
            }
            finally
            {
                Views.Busy.SetBusy(false);
            }
            return default(T);
        }
    }
}