using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JustTrek.Models;
using Template10.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JustTrek.Views
{

    public class MySelector : DataTemplateSelector
    {
        public DataTemplate FacebookTemplate { get; set; }
        public DataTemplate FlickrTemplate { get; set; }
        public DataTemplate RssTemplate { get; set; }
        public DataTemplate TwitterTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var group = item as Models.Group;
            switch (group.Kind)
            {
                case Kinds.Facebook:
                    return FacebookTemplate;
                case Kinds.TwitterUser:
                case Kinds.TwitterSearch:
                    return TwitterTemplate;
                case Kinds.FlickrId:
                case Kinds.FlickrTags:
                    return FlickrTemplate;
                case Kinds.Rss:
                    return RssTemplate;
                default:
                    return null;
            }
        }
    }

}