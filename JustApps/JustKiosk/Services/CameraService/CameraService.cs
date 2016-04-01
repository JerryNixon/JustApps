using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Utils;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace JustKiosk.Services.CameraService
{
    public class CameraService
    {
        public static CameraService Instance;

        static CameraService()
        {
            Instance = new CameraService();
        }

        private CameraService()
        {
            // Prevent the screen from sleeping 
            try
            {
                DisplayRequest.RequestActive();
            }
            catch { }
        }

        private DisplayRequest DisplayRequest { get; } = new DisplayRequest();
        public static MediaCapture DefaultManager { get; set; } = null;
        public string VideoId => DefaultManager?.MediaCaptureSettings.VideoDeviceId;
        public string AudioId => DefaultManager?.MediaCaptureSettings.VideoDeviceId;

        public async Task<MediaCapture> InitializeAsync(string videoDeviceId, string audioDeviceId = null)
        {
            if (DefaultManager != null && DefaultManager.MediaCaptureSettings.VideoDeviceId.ToLower() == videoDeviceId.ToLower())
                return DefaultManager;

            try
            {
                await StopEverything();
            }
            catch
            {
                throw;
            }

            DefaultManager = new MediaCapture();
            DefaultManager.RecordLimitationExceeded += DefaultManager_RecordLimitationExceeded;
            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
            };
            if (videoDeviceId != null) settings.VideoDeviceId = videoDeviceId;
            if (audioDeviceId != null) settings.AudioDeviceId = audioDeviceId;

            try
            {
                await DefaultManager.InitializeAsync(settings);
                await SetupFaceDetection();
            }
            catch
            {
                throw;
            }

            return DefaultManager;
        }

        private async void DefaultManager_RecordLimitationExceeded(MediaCapture sender)
        {
            await Video.StopAsync();
        }

        public async Task StopEverything()
        {
            if (Preview.Previewing)
                await Preview.StopAsync();

            if (Video.Capturing)
                await Video.StopAsync();

            if (FaceDetection != null)
            {
                FaceDetection.Enabled = false;
                FaceDetection.FaceDetected -= FaceDetection_FaceDetected;
            }

            if (DefaultManager != null)
            {
                DefaultManager.RecordLimitationExceeded -= DefaultManager_RecordLimitationExceeded;
                await DefaultManager.ClearEffectsAsync(MediaStreamType.VideoPreview);
                DefaultManager.Dispose();
                DefaultManager = null;
            }
        }

        #region Face detect

        public static event EventHandler<int> FaceCountChanged;
        public static int? FacesCount => (Instance?.FacesCanvas.Child as Canvas)?.Children.Count();

        public static FaceDetectionEffect FaceDetection;
        public event EventHandler<FaceDetectedEventArgs> FaceDetected;
        public Color FacesBoxColor { get; set; } = Colors.Yellow;
        public Viewbox FacesCanvas { get; } = new Viewbox
        {
            Child = new Canvas(),
            Stretch = Windows.UI.Xaml.Media.Stretch.Uniform,
        };

        private async Task<FaceDetectionEffect> SetupFaceDetection()
        {
            var faceDetectionEffectDefinition = new FaceDetectionEffectDefinition
            {
                SynchronousDetectionEnabled = false,
                DetectionMode = FaceDetectionMode.HighPerformance,
            };
            var faceDetection = await DefaultManager.AddVideoEffectAsync(faceDetectionEffectDefinition, MediaStreamType.VideoPreview) as FaceDetectionEffect;
            faceDetection.FaceDetected += FaceDetection_FaceDetected;
            faceDetection.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
            if (FaceDetection != null) FaceDetection.Enabled = false;
            return FaceDetection = faceDetection;
        }

        private void FaceDetection_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            DispatcherWrapper.Current().Dispatch(() =>
            {
                var properties = DefaultManager.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                if (properties == null || properties.Width == 0 || properties.Height == 0)
                    return;

                var canvas = FacesCanvas.Child as Canvas;
                canvas.Height = properties.Height;
                canvas.Width = properties.Width;
                FaceDetected?.Invoke(sender, args);
                canvas.Children.Clear();

                foreach (var face in args.ResultFrame.DetectedFaces.Where(x => x.FaceBox.Width != 0 && x.FaceBox.Height != 0))
                {
                    var box = new Rectangle
                    {
                        Height = face.FaceBox.Height,
                        Width = face.FaceBox.Width,
                        Stroke = FacesBoxColor.ToSolidColorBrush(),
                        StrokeThickness = 2,
                    };
                    Canvas.SetLeft(box, face.FaceBox.X);
                    Canvas.SetTop(box, face.FaceBox.Y);
                    canvas.Children.Add(box);
                }
                FaceCountChanged?.Invoke(this, canvas.Children.Count());
            });
        }

        #endregion

        PreviewClass _Preview;
        public PreviewClass Preview
        {
            get
            {
                if (_Preview != null)
                {
                    return _Preview;
                }
                return _Preview = new PreviewClass();
            }
        }

        PhotoClass _Photo;
        public PhotoClass Photo
        {
            get
            {
                if (_Photo != null)
                {
                    return _Photo;
                }
                return _Photo = new PhotoClass();
            }
        }

        VideoClass _Video;
        public VideoClass Video
        {
            get
            {
                if (_Video != null)
                {
                    return _Video;
                }
                return _Video = new VideoClass();
            }
        }
    }
}
