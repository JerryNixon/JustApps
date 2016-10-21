using System;
using Windows.UI.Xaml.Controls;

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

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ZoomedOutListView.ItemsSource = SessionsCVS.View.CollectionGroups;
            MySemanticZoom.IsZoomOutButtonEnabled = true;
            MySemanticZoom.IsZoomedInViewActive = false;
        }
    }
}
