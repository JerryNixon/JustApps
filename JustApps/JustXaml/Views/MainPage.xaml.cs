using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
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
            Loaded += (s, e) => Clear();
        }


        public async void Clear()
        {
            var uri = new Uri("ms-appx:///Templates/Default.txt");
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            RawXamlText = await FileIO.ReadTextAsync(file);
            Update();
        }

        public string RawXamlText { get { return (string)GetValue(RawXamlTextProperty); } set { SetValue(RawXamlTextProperty, value); } }
        public static readonly DependencyProperty RawXamlTextProperty =
            DependencyProperty.Register("RawXamlText", typeof(string), typeof(MainPage), new PropertyMetadata(string.Empty, (d, e) => (d as MainPage).Update()));

        public UIElement XamlContent { get { return (UIElement)GetValue(XamlContentProperty); } set { SetValue(XamlContentProperty, value); } }
        public static readonly DependencyProperty XamlContentProperty =
            DependencyProperty.Register("XamlContent", typeof(UIElement), typeof(MainPage), new PropertyMetadata(null));

        public string ErrorText { get { return (string)GetValue(ErrorTextProperty); } set { SetValue(ErrorTextProperty, value); } }
        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register("ErrorText", typeof(string), typeof(MainPage), new PropertyMetadata(string.Empty));

        private void Update()
        {
            if (PauseButton.IsChecked ?? false)
                return;
            if (string.IsNullOrWhiteSpace(RawXamlText))
            {
                ErrorText = "Xaml is empty";
            }
            else
            {
                ErrorText = string.Empty;
                try
                {
                    XamlContent = XamlReader.Load(RawXamlText) as UIElement;
                }
                catch (Exception ex)
                {
                    var a = Regex.Split(ex.Message, Environment.NewLine);
                    ErrorText = $"{a.Last()}";
                }
            }
        }

        private async void Template_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                var item = e.AddedItems.First() as Models.Item;
                var path = item.Path;
                var file = await StorageFile.GetFileFromPathAsync(path);
                RawXamlText = await FileIO.ReadTextAsync(file);
            }
        }
    }
}
