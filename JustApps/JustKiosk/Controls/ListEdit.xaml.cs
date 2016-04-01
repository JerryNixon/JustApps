using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace JustKiosk.Controls
{
    public sealed partial class ListEdit : UserControl
    {
        public ListEdit()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string),
                typeof(ListEdit), new PropertyMetadata(string.Empty));

        public ObservableCollection<string> ItemsSource
        {
            get { return (ObservableCollection<string>)GetValue(ItemsSourceProperty); }
            set
            {
                var list = ItemsSource;
                foreach (var item in value)
                {
                    list.Add(item);
                }
            }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<string>),
                typeof(ListEdit), new PropertyMetadata(new ObservableCollection<string>()));

        private string AddThis
        {
            get { return (string)GetValue(AddThisProperty); }
            set { SetValue(AddThisProperty, value); }
        }
        private static readonly DependencyProperty AddThisProperty =
            DependencyProperty.Register(nameof(AddThis), typeof(string),
                typeof(ListEdit), new PropertyMetadata(string.Empty));

        private void Add()
        {
            Uri uri;
            if (!Uri.TryCreate(AddThis, UriKind.Absolute, out uri))
                return;
            var host = uri.Host.ToLower().Replace("www.", string.Empty);

            if (!ItemsSource?.Any(x => x == host) ?? false)
                ItemsSource?.Add(host);

            AddThis = string.Empty;
            ListChanged.Invoke(this, EventArgs.Empty);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var b = (sender as Button);
            var i = b?.DataContext as string;
            ItemsSource?.Remove(i);
            ListChanged.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ListChanged;
    }
}
