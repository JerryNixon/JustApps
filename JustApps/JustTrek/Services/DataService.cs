using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AppStudio.DataProviders.Facebook;
using AppStudio.DataProviders.Flickr;
using AppStudio.DataProviders.Rss;
using AppStudio.DataProviders.Twitter;
using JustTrek.Models;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml.Controls;

namespace JustTrek.Services
{
    public partial class DataService
    {
        public readonly static DataService Instance;
        static DataService()
        {
            Instance = new DataService();
        }

        SettingsService _SettingSvc;
        private DataService()
        {
            _SettingSvc = new SettingsService();
        }

        public async Task<IEnumerable<Item>> GetFaceBookItemsAsync(string query, int max = 20)
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
            return items.Select(x => new Item(x));
        }

        public async Task FillAsync(Group group, Parameter parameter)
        {
            switch (group.Kind)
            {
                case Kinds.Facebook:
                    FillItems(group, await GetFaceBookItemsAsync(parameter.Param, parameter.Max));
                    break;
                case Kinds.TwitterUser:
                    FillItems(group, await GetTwitterItemsAsync(parameter.Param, TwitterQueryType.User, parameter.Max));
                    break;
                case Kinds.TwitterSearch:
                    FillItems(group, await GetTwitterItemsAsync(parameter.Param, TwitterQueryType.Search, parameter.Max));
                    break;
                case Kinds.FlickrId:
                    FillItems(group, await GetFlickrItemsAsync(parameter.Param, FlickrQueryType.Id, parameter.Max));
                    break;
                case Kinds.FlickrTags:
                    FillItems(group, await GetFlickrItemsAsync(parameter.Param, FlickrQueryType.Tags, parameter.Max));
                    break;
                case Kinds.Rss:
                    FillItems(group, await GetRssItemsAsync(new Uri(parameter.Param), parameter.Max));
                    break;
                default:
                    throw new NotSupportedException(group.Kind.ToString());
            }
            group.Items.ForEach(x => x.Kind = group.Kind);
        }

        private void FillItems(Group group, IEnumerable<Item> items)
        {
            if (items != null && items.Any())
            {
                group.Items.AddRange(items, true);
            }
            else
            {
                // TODO: handle error
            }
        }

        internal async Task<IEnumerable<Item>> GetRssItemsAsync(Uri url, int max = 20)
        {
            var items = await ExecuteAsync(async () =>
            {
                var config = new RssDataConfig
                {
                    Url = url,
                };
                var rssDataProvider = new RssDataProvider();
                return await rssDataProvider.LoadDataAsync(config, max);
            });
            return items.Select(x => new Item(x));
        }

        public async Task<IEnumerable<Item>> GetFlickrItemsAsync(string param, FlickrQueryType type = FlickrQueryType.Tags, int max = 20)
        {
            var items = await ExecuteAsync(async () =>
            {
                var config = new FlickrDataConfig
                {
                    Query = param,
                    QueryType = type
                };
                var flickrDataProvider = new FlickrDataProvider();
                return await flickrDataProvider.LoadDataAsync(config, max);
            });
            return items.Select(x => new Item(x));
        }

        public async Task<IEnumerable<Item>> GetTwitterItemsAsync(string query, TwitterQueryType type, int max = 20)
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
            return items.Select(x => new Item(x));
        }

        async Task<T> ExecuteAsync<T>(Func<Task<T>> action, [CallerMemberName]string name = null)
        {
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
            return default(T);
        }
    }
}