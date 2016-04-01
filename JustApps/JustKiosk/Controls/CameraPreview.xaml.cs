using System;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustKiosk.Controls
{
    public sealed partial class CameraPreview : UserControl
    {
        public CameraPreview()
        {
            this.InitializeComponent();
        }

        public string VideoId
        {
            get { return (string)GetValue(VideoIdProperty); }
            set { SetValue(VideoIdProperty, value); }
        }
        public static readonly DependencyProperty VideoIdProperty =
            DependencyProperty.Register(nameof(VideoId), typeof(string),
                typeof(CameraPreview), new PropertyMetadata(null));

        private DeviceInformation SelectedCamera
        {
            get { return (DeviceInformation)GetValue(SelectedCameraProperty); }
            set { SetValue(SelectedCameraProperty, value); }
        }
        private static readonly DependencyProperty SelectedCameraProperty =
            DependencyProperty.Register(nameof(SelectedCamera), typeof(DeviceInformation),
                typeof(CameraPreview), new PropertyMetadata(null, SelectedCameraChanged));
        private static async void SelectedCameraChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CameraPreview;
            var device = e.NewValue as DeviceInformation;
            var deviceId = device.Id;
            var cameraService = Services.CameraService.CameraService.Instance;
            await cameraService.InitializeAsync(deviceId);
            if (!cameraService.Preview.Previewing)
                await cameraService.Preview.StartAsync(c.MyCaptureElement);
        }

        private async void MyCaptureElement_Loaded(object sender, RoutedEventArgs args)
        {
            var cameraService = Services.CameraService.CameraService.Instance;
            MyCaptureGrid.Children.Add(cameraService.FacesCanvas);

            var deviceService = Services.DeviceService.Instance;
            await deviceService.InitializeAsync();

            var setup = new Action<string>(id =>
            {
                CameraComboBox.ItemsSource = deviceService.Cameras;
                SelectedCamera = deviceService.GetCamera(id);

                if (!deviceService.Cameras.Any())
                {
                    CameraNotFoundTextBlock.Visibility = Visibility.Visible;
                    CameraComboBox.Visibility = Visibility.Collapsed;
                }
                else if (deviceService.Cameras.Count == 1)
                {
                    CameraNotFoundTextBlock.Visibility = Visibility.Collapsed;
                    CameraComboBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CameraNotFoundTextBlock.Visibility = Visibility.Collapsed;
                    CameraComboBox.Visibility = Visibility.Visible;
                }
            });

            var settings = Services.SettingsService.Instance;
            setup(null);

            deviceService.CamerasChanged += (s, e) =>
            {
                var selected = CameraComboBox.SelectedItem as DeviceInformation;
                var selectedId = selected?.Id;
                setup(selectedId);
            };

            App.Current.Suspending += async (s, e) =>
            {
                await cameraService.StopEverything();
            };
            App.Current.Resuming += (s, e) =>
            {
                var selected = CameraComboBox.SelectedItem as DeviceInformation;
                var selectedId = selected?.Id;
                setup(selectedId);
            };
        }
    }
}
