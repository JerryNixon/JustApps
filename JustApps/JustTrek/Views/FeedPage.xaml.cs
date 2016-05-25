using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var group = item as Models.Group;
            return group.Template;
        }
    }

    public sealed partial class FeedPage : Page
    {
        Services.DataService _DataService;
        public FeedPage()
        {
            InitializeComponent();
            Loaded += FeedPage_Loaded;
            Update(Window.Current.Bounds.Width);
            SizeChanged += (s, e) =>
            {
                Update(e.NewSize.Width);
            };
        }

        void Update(double width)
        {
            PageWidth = width;
            ColumnCount = (int)(width / ColumnWidth);
        }

        private void FeedPage_Loaded(object sender, RoutedEventArgs e)
        {
            // AllGroupsZoomedOutListView.ItemsSource = CVS.View.CollectionGroups;
            _DataService = new Services.DataService();

            AddAsync(Kinds.Rss, new Parameter { Title = "StarTrek.com", Param = "http://www.startrek.com/latest_news_feed" });
            AddAsync(Kinds.Rss, new Parameter { Title = "TrekToday.com", Param = "http://www.trektoday.com/feed" });
            AddAsync(Kinds.Rss, new Parameter { Title = "TrekMovie.com", Param = "http://trekmovie.com/feed" });
            AddAsync(Kinds.Rss, new Parameter { Title = "TrekNews.net", Param = "http://www.treknews.net/feed/" });


            AddAsync(Kinds.Facebook, new Parameter { Title = "Star Trek", Param = "StarTrek" });
            AddAsync(Kinds.Facebook, new Parameter { Title = "Star Trek Movie", Param = "StarTrekMovie" });

            AddAsync(Kinds.TwitterUser, new Parameter { Title = "Star Trek", Param = "StarTrek" });
            AddAsync(Kinds.TwitterUser, new Parameter { Title = "Star Trek", Param = "StarTrekMovie" });

            AddAsync(Kinds.FlickrTags, new Parameter { Title = "Star Trek", Param = "StarTrek" });
            AddAsync(Kinds.FlickrTags, new Parameter { Title = "Star Trek", Param = "StarTrekMovie" });

            Busy.SetBusy(false);
        }

        private async void AddAsync(Kinds kind, Parameter parameter, int max = 20)
        {
            var group = Groups.AddAndReturn(new Group { Title = parameter.Title });
            IEnumerable<Item> items = null;
            switch (kind)
            {
                case Kinds.Facebook:
                    group.Template = FacebookItemTemplate;
                    group.Image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Facebook.png";
                    items = await _DataService.GetFaceBookItemsAsync(parameter.Param, max);
                    break;
                case Kinds.TwitterUser:
                    group.Template = TwitterItemTemplate;
                    group.Image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Twitter.png";
                    items = await _DataService.GetTwitterItemsAsync(parameter.Param, AppStudio.DataProviders.Twitter.TwitterQueryType.User, max);
                    break;
                case Kinds.TwitterSearch:
                    group.Template = TwitterItemTemplate;
                    group.Image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Twitter.png";
                    items = await _DataService.GetTwitterItemsAsync(parameter.Param, AppStudio.DataProviders.Twitter.TwitterQueryType.Search, max);
                    break;
                case Kinds.FlickrId:
                    group.Template = FlickrItemTemplate;
                    group.Image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Flickr.png";
                    items = await _DataService.GetFlickrItemsAsync(parameter.Param, AppStudio.DataProviders.Flickr.FlickrQueryType.Id, max);
                    break;
                case Kinds.FlickrTags:
                    group.Template = FlickrItemTemplate;
                    group.Image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Flickr.png";
                    items = await _DataService.GetFlickrItemsAsync(parameter.Param, AppStudio.DataProviders.Flickr.FlickrQueryType.Tags, max);
                    break;
                case Kinds.Rss:
                    group.Template = RssItemTemplate;
                    group.Image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/Rss.png";
                    items = await _DataService.GetRssItemsAsync(new Uri(parameter.Param), max);
                    break;
            }
            if (items != null && items.Any())
            {
                group.Items.AddRange(items, true);
            }
            else
            {
                // handle error
            }
        }

        public void ItemClick() { }

        public ObservableCollection<Models.Group> Groups { get; } = new ObservableCollection<Models.Group>();

        public DataTemplate FacebookItemTemplate
        {
            get { return (DataTemplate)GetValue(FacebookItemTemplateProperty); }
            set { SetValue(FacebookItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty FacebookItemTemplateProperty =
            DependencyProperty.Register("FacebookItemTemplate", typeof(DataTemplate), typeof(FeedPage), new PropertyMetadata(null));

        public DataTemplate TwitterItemTemplate
        {
            get { return (DataTemplate)GetValue(TwitterItemTemplateProperty); }
            set { SetValue(TwitterItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty TwitterItemTemplateProperty =
            DependencyProperty.Register("TwitterItemTemplate", typeof(DataTemplate), typeof(FeedPage), new PropertyMetadata(null));
        public DataTemplate FlickrItemTemplate
        {
            get { return (DataTemplate)GetValue(FlickrItemTemplateProperty); }
            set { SetValue(FlickrItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty FlickrItemTemplateProperty =
            DependencyProperty.Register("FlickrItemTemplate", typeof(DataTemplate), typeof(FeedPage), new PropertyMetadata(null));

        public DataTemplate RssItemTemplate
        {
            get { return (DataTemplate)GetValue(RssItemTemplateProperty); }
            set { SetValue(RssItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty RssItemTemplateProperty =
            DependencyProperty.Register("RssItemTemplate", typeof(DataTemplate), typeof(FeedPage), new PropertyMetadata(null));

        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(double), typeof(FeedPage), new PropertyMetadata(300d));

        public double ColumnCount
        {
            get { return (double)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(double), typeof(FeedPage), new PropertyMetadata(300d));

        public double PageWidth
        {
            get { return (double)GetValue(PageWidthProperty); }
            set { SetValue(PageWidthProperty, value); }
        }
        public static readonly DependencyProperty PageWidthProperty =
            DependencyProperty.Register("PageWidth", typeof(double), typeof(FeedPage), new PropertyMetadata(300d));
    }
}
