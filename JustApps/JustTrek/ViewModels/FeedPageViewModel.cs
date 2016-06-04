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
                Groups.AddAndReturn(new Group { Kind = Kinds.Rss, Title = "StarTrek.com", Parameter = new Parameter { Param = "http://www.startrek.com/latest_news_feed" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.Facebook, Title = "Star Trek", Parameter = new Parameter { Param = "StarTrek" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.TwitterUser, Title = "Star Trek", Parameter = new Parameter { Param = "StarTrek" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.Facebook, Title = "Star Trek Movie", Parameter = new Parameter { Param = "StarTrekMovie" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.TwitterUser, Title = "Star Trek", Parameter = new Parameter { Param = "StarTrekMovie" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.Rss, Title = "TrekToday.com", Parameter = new Parameter { Param = "http://www.trektoday.com/feed" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.Rss, Title = "TrekMovie.com", Parameter = new Parameter { Param = "http://trekmovie.com/feed" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.Rss, Title = "TrekNews.net", Parameter = new Parameter { Param = "http://www.treknews.net/feed/" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.FlickrTags, Title = "Star Trek", Parameter = new Parameter { Param = "StarTrek" } }).Fill();
                Groups.AddAndReturn(new Group { Kind = Kinds.FlickrTags, Title = "Star Trek", Parameter = new Parameter { Param = "StarTrekMovie" } }).Fill();
            }
        }

        public ObservableCollection<Models.Group> Groups { get; private set; } = new ObservableCollection<Models.Group>();
    }
}
