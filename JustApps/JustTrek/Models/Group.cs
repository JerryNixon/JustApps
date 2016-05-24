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
using Windows.UI.Xaml;

namespace JustTrek.Models
{
    public class Group
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public Uri Link { get; set; }
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();
        public DataTemplate Template { get; internal set; }
    }
}