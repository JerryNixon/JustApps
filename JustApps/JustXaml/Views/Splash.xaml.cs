﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace JustXaml.Views
{
    public sealed partial class Splash : Page
    {
        public Splash(SplashScreen splashScreen)
        {
            InitializeComponent();
            Window.Current.SizeChanged += (s, e) => Resize(splashScreen);
            Resize(splashScreen);
        }

        private void Resize(SplashScreen splashScreen)
        {
            if (splashScreen.ImageLocation.Top == 0)
            {
                splashImage.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                rootCanvas.Background = null;
                splashImage.Visibility = Visibility.Visible;
            }
            splashImage.Height = splashScreen.ImageLocation.Height;
            splashImage.Width = splashScreen.ImageLocation.Width;
            splashImage.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top);
            splashImage.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left);
        }
    }
}
