using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JustTrek.Models;
using Template10.Utils;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JustTrek.Models
{
    public enum Statuses { Loading, Loaded }
    public enum Kinds { Facebook, TwitterUser, TwitterSearch, FlickrId, FlickrTags, Rss }
}