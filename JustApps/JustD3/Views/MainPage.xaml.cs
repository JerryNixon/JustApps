using System;
using Template10.Common;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JustD3.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusbar.ShowAsync();
                statusbar.BackgroundColor = Colors.SteelBlue;
                statusbar.ForegroundColor = Colors.White;
                statusbar.BackgroundOpacity = 1;
            }

            ZoomedOutListView.ItemsSource = SessionsCVS.View.CollectionGroups;
            MySemanticZoom.IsZoomOutButtonEnabled = true;
            MySemanticZoom.IsZoomedInViewActive = false;
        }

        private async void ZoomedInListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var session = e.ClickedItem as Models.Session;

            var view = new WebView(WebViewExecutionMode.SameThread)
            {
                Width = 350,
                MinHeight = 500,
                Margin = new Thickness(0),
                DefaultBackgroundColor = Colors.Transparent,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            view.NavigateToString("<body style='font-family:Segoe UI Light;'>" + session.Description);

            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Width = 380,
                Text = session.Title,
                Margin = new Thickness(0, 16, 0, 0),
                TextWrapping = TextWrapping.Wrap,
            });
            stack.Children.Add(view);

            var dialog = new ContentDialog
            {
                Content = stack,
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = "Close",
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };
            await dialog.ShowAsync();
        }

        private void Favorite_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.AddFavorite((sender as FrameworkElement).DataContext as Models.Session);
        }

        private void Favorite_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveFavorite((sender as FrameworkElement).DataContext as Models.Session);
        }

        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TryResizeView(new Size { Height = 768, Width = 1366 });
        }
    }
}
