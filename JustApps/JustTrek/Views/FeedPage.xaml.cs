using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JustTrek.Models;
using Template10.Utils;
using Windows.Foundation;
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
                WebView.Visibility = Visibility.Collapsed;
            };
            WebView.NavigationCompleted += (s, e) =>
            {
                WebProgressRing.IsActive = false;
                WebView.Visibility = Visibility.Visible;
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
            WebView.Navigate(link);
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
            //var buttons = XamlUtils.AllChildren<Button>(section);
            //if (buttons.Any())
            //{
            //    var button = buttons.First(x => x.Name == "SeeMoreButton");
            //    button.Content = "Click to see site";
            //}
            RootHub.Sections.Add(section);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Item;
            Navigate(item.Link);
            VisualStateManager.GoToState(this, WebVisualState.Name, true);
        }

        Uri blank = new Uri("about:blank");
        private void LayoutVisualStateGroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == ListVisualState)
            {
                Navigate(blank);
            }
        }

        private void RootView_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView == false)
            {
                var section = RootHub.Sections.First(x => x.DataContext == e.SourceItem.Item);
                RootHub.ScrollToSection(section);
            }
        }

        private void ScollHubToSection(HubSection section)
        {
            var visual = section.TransformToVisual(RootHub);
            var point = visual.TransformPoint(new Point(0, 0));
            var viewer = RootHub.AllChildren().First(x => x is ScrollViewer) as ScrollViewer;
            viewer.ChangeView(point.X, null, null);
        }
    }
}
