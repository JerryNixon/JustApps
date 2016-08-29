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
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var data = item as Models.Group;
            switch (data.Kind)
            {
                case Kinds.Facebook:
                    return FacebookTemplate ?? DefaultTemplate;
                case Kinds.TwitterUser:
                case Kinds.TwitterSearch:
                    return TwitterTemplate ?? DefaultTemplate;
                case Kinds.FlickrId:
                case Kinds.FlickrTags:
                    return FlickrTemplate ?? DefaultTemplate;
                case Kinds.Rss:
                    return RssTemplate ?? DefaultTemplate;
                default:
                    return DefaultTemplate;
            }
        }
    }

}