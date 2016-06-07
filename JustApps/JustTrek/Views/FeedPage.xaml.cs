using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JustTrek.Models;
using Template10.Utils;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JustTrek.Views
{
    public sealed partial class FeedPage : Page
    {
        public FeedPage()
        {
            InitializeComponent();
            Loaded += FeedPage_Loaded;
            SizeChanged += (s, e) =>
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = $"W:{e.NewSize.Width} H:{e.NewSize.Height}";
            };
            WebView.NavigationStarting += (s, e) =>
            {
                WebProgressRing.IsActive = true;
            };
            WebView.NavigationCompleted += (s, e) =>
            {
                WebProgressRing.IsActive = false;
            };
        }

        private void FeedPage_Loaded(object sender, RoutedEventArgs args)
        {
            RootHub.Sections.Clear();
            RootHub.SectionHeaderClick += (s, e) =>
            {
                var group = e.Section.DataContext as Group;
                Navigate(group.Link);
                VisualStateManager.GoToState(this, WebVisualState.Name, true);
            };
            ViewModel.Groups.CollectionChanged += (s, e) =>
            {
                foreach (var group in e.NewItems?.Cast<Group>())
                {
                    AddSection(group);
                }
            };
            foreach (var group in ViewModel.Groups.ToArray())
            {
                AddSection(group);
            }
        }

        private void Navigate(Uri link)
        {
            if (WebView.Source != link)
            {
                WebView.Navigate(new Uri("about:blank"));
                WebView.Navigate(link);
            }
        }

        private void AddSection(Group group)
        {
            var section = new HubSection
            {
                HeaderTemplate = HeaderTemplate,
                ContentTemplate = SectionTemplate,
                DataContext = group,
                IsHeaderInteractive = true,
            };
            RootHub.Sections.Add(section);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Item;
            Navigate(item.Link);
            VisualStateManager.GoToState(this, WebVisualState.Name, true);
        }

        private void LayoutVisualStateGroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == ListVisualState)
                Navigate(new Uri("about:blank"));
        }
    }
}
