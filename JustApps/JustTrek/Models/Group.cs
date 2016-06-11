using System;
using System.Linq;
using System.Collections.ObjectModel;
using Template10.Utils;
using Template10.Mvvm;

namespace JustTrek.Models
{
    public class Group : BindableBase
    {
        public string Title { get; set; }
        public string Image
        {
            get
            {
                switch (Kind)
                {
                    case Kinds.Facebook:
                        return "ms-appx:///Images/Facebook.png";
                    case Kinds.TwitterUser:
                    case Kinds.TwitterSearch:
                        return "ms-appx:///Images/Twitter.png";
                    case Kinds.FlickrId:
                    case Kinds.FlickrTags:
                        return "ms-appx:///Images/Flickr.png";
                    case Kinds.Rss:
                        return "ms-appx:///Images/Rss.png";
                    default:
                        return "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/T10%20256x256.png";
                }
            }

        }
        public Uri Link { get; set; }
        public Kinds Kind { get; set; }
        public Parameter Parameter { get; set; }
        public ObservableCollection<Item> Items { get; private set; } = new ObservableCollection<Item>();

        Statuses _Status = default(Statuses);
        public Statuses Status { get { return _Status; } set { Set(ref _Status, value); } }

        DelegateCommand _RefreshCommand;
        public DelegateCommand RefreshCommand => _RefreshCommand ?? (_RefreshCommand = new DelegateCommand(Fill));

        public async void Fill()
        {
            Items.Clear();
            Status = Statuses.Loading;
            await Services.DataService.Instance.FillAsync(this, Parameter);
            Status = Statuses.Loaded;
        }

        public static Group GetSample(string title, Kinds? kind = null) => new Group
        {
            Title = title ?? "My Group",
            Kind = kind ?? Kinds.Rss,
            Link = new Uri("http://bing.com"),
            Items = Enumerable.Range(1, 5).Select(x => new Item
            {
                Title = "The quick brown fox jumps over the lazy dog",
                Link = new Uri("http://bing.com"),
                Date = DateTime.Now.ToString(),
                Kind = kind.Value,
                Image = "https://raw.githubusercontent.com/Windows-XAML/Template10/master/Assets/T10%20256x256.png",
                Details = "The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog",
            }).ToObservableCollection()
        };
    }
}