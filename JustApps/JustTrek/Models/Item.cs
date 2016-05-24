using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppStudio.DataProviders.Facebook;
using JustTrek.Models;
using Template10.Utils;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AppStudio.DataProviders.Twitter;
using AppStudio.DataProviders.Flickr;
using AppStudio.DataProviders.Rss;

namespace JustTrek.Models
{
    public class Item
    {
        public Item(FacebookSchema x)
        {
            Date = x.PublishDate.ToString("D");
            Title = x.Title;
            Image = x.ImageUrl;
            Details = x.Summary;
            Link = new Uri(x.FeedUrl);
        }

        public Item(TwitterSchema x)
        {
            Date = x.CreationDateTime.ToString("D");
            Title = x.Text;
            Image = x.UserProfileImageUrl;
            Details = string.Empty;
            Link = new Uri(x.Url);
        }

        public Item(FlickrSchema x)
        {
            Date = x.Published.ToString("D");
            Title = x.Title;
            Image = x.ImageUrl;
            Details = x.Summary;
            Link = new Uri(x.FeedUrl);
        }

        public Item(RssSchema x)
        {
            Date = x.PublishDate.ToString("D");
            Title = x.Title;
            Image = x.ImageUrl;
            Details = x.Summary;
            Link = new Uri(x.FeedUrl);
        }

        public string Date { get; }
        public string Title { get; }
        public string Image { get; }
        public string Details { get; }
        public Uri Link { get; }
    }
}