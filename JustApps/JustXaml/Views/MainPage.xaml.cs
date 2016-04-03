using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.LockScreen;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace JustXaml.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            AllSamplesCollectionViewSource.Source = ViewModel.SamplePane.UnfilteredSamples;
            AllSamplesZoomedOutListView.ItemsSource = AllSamplesCollectionViewSource.View.CollectionGroups;
        }

        private void AllSampleSemanticZoomButton_Click(object sender, RoutedEventArgs e)
        {
            AllSamplesSemanticZoom.ToggleActiveView();
        }

        private void GoToState_Click(object sender, RoutedEventArgs e)
        {
            var stateName = (sender as FrameworkElement).Tag as string;
            VisualStateManager.GoToState(this, stateName, true);
        }
    }
}
