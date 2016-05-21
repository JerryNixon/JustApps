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

namespace JustTrek.Models
{
    public class Group
    {
        public string Title { get; set; }
        public IEnumerable<object> Items { get; set; }
    }
}