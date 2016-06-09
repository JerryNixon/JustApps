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
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.Rss,
                    Title = "Official News from StarTrek.com",
                    Link = new Uri("http://startrek.com"),
                    Parameter = new Parameter { Param = "http://www.startrek.com/latest_news_feed" }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.Facebook,
                    Title = "Official Star Trek on Facebook",
                    Link = new Uri("http://facebook.com/StarTrek"),
                    Parameter = new Parameter { Param = "StarTrek" }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.TwitterUser,
                    Title = "Official Star Trek Twitter Feed",
                    Link = new Uri("http://twitter.com/StarTrek"),
                    Parameter = new Parameter { Param = "StarTrek", Max = 10 }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.Facebook,
                    Title = "Official Star Trek Movie on Facebook",
                    Link = new Uri("http://facebook.com/StarTrekMovie"),
                    Parameter = new Parameter { Param = "StarTrekMovie" }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.TwitterUser,
                    Title = "Official Star Trek on Twitter",
                    Link = new Uri("http://twitter.com/StarTrekMovie"),
                    Parameter = new Parameter { Param = "StarTrekMovie", Max = 10 }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.Rss,
                    Title = "News from TrekToday.com",
                    Link = new Uri("http://TrekToday.com"),
                    Parameter = new Parameter { Param = "http://www.trektoday.com/feed" }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.Rss,
                    Title = "News from TrekCore.com",
                    Link = new Uri("http://TrekCore.com"),
                    Parameter = new Parameter { Param = "http://TrekCore.com/feed.xml" }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.Rss,
                    Title = "News from TrekMovie.com",
                    Link = new Uri("http://trekmovie.com"),
                    Parameter = new Parameter { Param = "http://trekmovie.com/feed" }
                }).Fill();
                Groups.AddAndReturn(new Group
                {
                    Kind = Kinds.Rss,
                    Title = "News from TrekNews.net",
                    Link = new Uri("http://treknews.net"),
                    Parameter = new Parameter { Param = "http://www.treknews.net/feed/" }
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
            }
        }

        public ObservableCollection<Models.Group> Groups { get; private set; } = new ObservableCollection<Models.Group>();
    }
}
