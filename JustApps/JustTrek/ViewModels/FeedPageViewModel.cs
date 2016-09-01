using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustTrek.Models;
using Template10.Mvvm;
using Template10.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace JustTrek.ViewModels
{
    public class FeedPageViewModel : ViewModelBase
    {
        public FeedPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Groups.Add(Group.GetSample("RSS Group", Kinds.Rss));
                Groups.Add(Group.GetSample("Twitter Group", Kinds.TwitterUser));
                Groups.Add(Group.GetSample("Facebook Group", Kinds.Facebook));
                Groups.Add(Group.GetSample("Flickr Group", Kinds.FlickrTags));
            }
            else
            {
                Fill();
            }
        }

        private void Fill()
        {
            Groups.Clear();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Rss,
                Title = "StarTrek.com News",
                Link = new Uri("http://startrek.com"),
                Parameter = new Parameter { Param = "http://www.startrek.com/latest_news_feed" }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Facebook,
                Title = "Star Trek from Facebook",
                Link = new Uri("http://facebook.com/StarTrek"),
                Parameter = new Parameter { Param = "StarTrek" }
            }).Fill();
            //Groups.AddAndReturn(new Group
            //{
            //    Kind = Kinds.TwitterUser,
            //    Title = "Official Star Trek Twitter Feed",
            //    Link = new Uri("http://twitter.com/StarTrek"),
            //    Parameter = new Parameter { Param = "StarTrek", Max = 10 }
            //}).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Facebook,
                Title = "Star Trek Movie from Facebook",
                Link = new Uri("http://facebook.com/StarTrekMovie"),
                Parameter = new Parameter { Param = "StarTrekMovie" }
            }).Fill();
            //Groups.AddAndReturn(new Group
            //{
            //    Kind = Kinds.TwitterUser,
            //    Title = "Official Star Trek on Twitter",
            //    Link = new Uri("http://twitter.com/StarTrekMovie"),
            //    Parameter = new Parameter { Param = "StarTrekMovie", Max = 10 }
            //}).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Rss,
                Title = "Star Trek Collective",
                Link = new Uri("http://www.thetrekcollective.com/"),
                Parameter = new Parameter { Param = "http://www.thetrekcollective.com/feeds/posts/default" }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Rss,
                Title = "TrekToday.com News",
                Link = new Uri("http://TrekToday.com"),
                Parameter = new Parameter { Param = "http://www.trektoday.com/feed" }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Rss,
                Title = "TrekCore.com News",
                Link = new Uri("http://TrekCore.com"),
                Parameter = new Parameter { Param = "http://TrekCore.com/feed.xml" }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Rss,
                Title = "TrekMovie.com News",
                Link = new Uri("http://trekmovie.com"),
                Parameter = new Parameter { Param = "http://trekmovie.com/feed" }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Rss,
                Title = "TrekNews.net News",
                Link = new Uri("http://treknews.net"),
                Parameter = new Parameter { Param = "http://www.treknews.net/feed/" }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Facebook,
                Title = "Trekyards on Facebook",
                Link = new Uri("https://www.facebook.com/groups/trekyards/"),
                Parameter = new Parameter { Param = "trekyards" }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.FlickrTags,
                Title = "Star Trek search on Flickr",
                Link = new Uri("https://www.flickr.com/search/?text=startrek"),
                Parameter = new Parameter { Param = "StarTrek", Max = 40 }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.FlickrTags,
                Title = "Star Trek Movie search on Flickr",
                Link = new Uri("https://www.flickr.com/search/?text=startrekmovie"),
                Parameter = new Parameter { Param = "StarTrekMovie", Max = 40 }
            }).Fill();
            Groups.AddAndReturn(new Group
            {
                Kind = Kinds.Rss,
                Title = "Memory Alpha News",
                Link = new Uri("http://http://memory-alpha.wikia.com/"),
                Parameter = new Parameter { Param = "http://memory-alpha.wikia.com/wiki/Special:NewPages?feed=rss" }
            }).Fill();
        }

        public void Refresh()
        {
            SelectedItem.RefreshCommand.Execute(null);
        }

        public ObservableCollection<Models.Group> Groups { get; private set; } = new ObservableCollection<Models.Group>();

        Models.Group _SelectedItem = default(Models.Group);
        public Models.Group SelectedItem { get { return _SelectedItem; } set { Set(ref _SelectedItem, value); } }
    }
}
