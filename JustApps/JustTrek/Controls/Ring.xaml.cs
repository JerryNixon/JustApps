using System;
using System.Collections.Generic;
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

namespace JustTrek.Controls
{
    public sealed partial class Ring : UserControl
    {
        public Ring()
        {
            this.InitializeComponent();
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(Ring), new PropertyMetadata(false, IsActiveChanged));
        private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                (d as Ring).Visibility = Visibility.Visible;
                (d as Ring).ActiveStoryboard.Begin();
            }
            else
            {
                (d as Ring).Visibility = Visibility.Collapsed;
                (d as Ring).ActiveStoryboard.Stop();
            }
        }
    }
}
