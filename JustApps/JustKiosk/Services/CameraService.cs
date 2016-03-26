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
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace JustKiosk.Services
{
    public class CameraService
    {
        string _SubFolderName;
        CaptureElement _captureElement;

        public DeviceInformationCollection Cameras { get; private set; }
        public event EventHandler CamerasChanged;

        public DeviceInformationCollection Microphones { get; private set; }
        public event EventHandler MicrophonesChanged;

        public event EventHandler<int> FaceDetected;

        public CameraService(CaptureElement element, string subfolder)
        {
            _captureElement = element;
            _SubFolderName = subfolder;
        }

        public bool Initialized
        {
            get
            {
                if (Cameras == null || Microphones == null)
                    return false;
                return Cameras.Union(Microphones).Any();
            }
        }

        public async Task InitializeAsync()
        {
            if (Initialized)
                throw new Exception("Already initialized");
            Cameras = await GetCamerasAsync(() => CamerasChanged?.Invoke(this, EventArgs.Empty));
            Microphones = await GetMicrophonesAsync(() => MicrophonesChanged?.Invoke(this, EventArgs.Empty));
        }

        DeviceWatcher CameraWatcher;
        Action CameraWatcherCallback;
        private async Task<DeviceInformationCollection> GetCamerasAsync(Action callback)
        {
            if (callback != null)
            {
                CameraWatcherCallback = callback;
                if (CameraWatcher == null)
                {
                    CameraWatcher = DeviceInformation.CreateWatcher(DeviceClass.VideoCapture);
                    CameraWatcher.Added += (s, e) => CameraWatcherCallback();
                    CameraWatcher.Removed += (s, e) => CameraWatcherCallback();
                    CameraWatcher.Updated += (s, e) => CameraWatcherCallback();
                    CameraWatcher.Start();
                }
            }

            return await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        }

        DeviceWatcher AudioWatcher;
        Action AudioWatcherCallback;
        private async Task<DeviceInformationCollection> GetMicrophonesAsync(Action callback)
        {
            if (callback != null)
            {
                AudioWatcherCallback = callback;
                if (AudioWatcher == null)
                {
                    AudioWatcher = DeviceInformation.CreateWatcher(DeviceClass.AudioCapture);
                    AudioWatcher.Added += (s, e) => AudioWatcherCallback();
                    AudioWatcher.Removed += (s, e) => AudioWatcherCallback();
                    AudioWatcher.Updated += (s, e) => AudioWatcherCallback();
                    AudioWatcher.Start();
                }
            }
            return await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
        }

        Dictionary<string, MediaCapture> Managers = new Dictionary<string, MediaCapture>();
        Dictionary<string, FaceDetectionEffect> FaceDetections = new Dictionary<string, FaceDetectionEffect>();
        public async Task<MediaCapture> GetManagerAsync(DeviceInformation videoDevice, DeviceInformation audioDevice)
        {
            if (Managers.ContainsKey(videoDevice?.Id ?? string.Empty))
            {
                return Managers[videoDevice.Id];
            }

            var manager = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
            };
            if (videoDevice != null)
            {
                settings.VideoDeviceId = videoDevice.Id;
            }
            if (audioDevice != null)
            {
                settings.AudioDeviceId = audioDevice.Id;
            }
            await manager.InitializeAsync(settings);
            manager.RecordLimitationExceeded += async (e) => await StopCaptureVideoAsync(videoDevice, audioDevice);

            // ensure race condition
            if (Managers.ContainsKey(manager.MediaCaptureSettings.VideoDeviceId))
            {
                manager.Dispose();
                return Managers[manager.MediaCaptureSettings.VideoDeviceId];
            }
            else
            {
                Managers.Add(manager.MediaCaptureSettings.VideoDeviceId, manager);
            }

            // setup face detection
            var faceDetectionEffectDefinition = new FaceDetectionEffectDefinition
            {
                SynchronousDetectionEnabled = false,
                DetectionMode = FaceDetectionMode.HighPerformance,
            };
            var faceDetection = await manager.AddVideoEffectAsync(faceDetectionEffectDefinition, MediaStreamType.VideoPreview) as FaceDetectionEffect;
            faceDetection.FaceDetected += (s, e) =>
            {
                DispatcherWrapper.Current().Dispatch(() =>
                {
                    FacesCanvas.Children.Clear();
                    FaceDetected?.Invoke(s, e.ResultFrame.DetectedFaces.Count());
                    var portrait = DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Portrait;
                    portrait = portrait ? true : DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.PortraitFlipped;
                    foreach (var face in e.ResultFrame.DetectedFaces.Where(x => x.FaceBox.Width != 0 && x.FaceBox.Height != 0))
                    {
                        var box = new Rectangle
                        {
                            Height = face.FaceBox.Height,
                            Width = face.FaceBox.Width,
                            Stroke = FacesBoxColor.ToSolidColorBrush(),
                            StrokeThickness = 2,
                        };
                        var x = face.FaceBox.X;
                        var y = face.FaceBox.Y;
                        Canvas.SetLeft(box, x);
                        Canvas.SetTop(box, y);
                    }
                });
            };
            faceDetection.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
            FaceDetections.Add(manager.MediaCaptureSettings.VideoDeviceId, faceDetection);
            return manager;
        }

        public Color FacesBoxColor { get; set; } = Colors.Yellow;
        public Canvas FacesCanvas { get; } = new Canvas();

        public async Task<VideoEncodingProperties> StartPreviewAsync(DeviceInformation camera = null, DeviceInformation microphone = null)
        {
            if (!Initialized)
                throw new Exception("Not initialized");
            var manager = await GetManagerAsync(camera, microphone);
            _captureElement.Source = manager;
            await manager.StartPreviewAsync();
            FaceDetections[manager.MediaCaptureSettings.VideoDeviceId].Enabled = true;
            return manager.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
        }

        public async Task StopPreviewAsync(DeviceInformation camera = null, DeviceInformation microphone = null)
        {
            if (!Initialized)
                throw new Exception("Not initialized");
            var manager = await GetManagerAsync(camera, microphone);
            await manager.StopPreviewAsync();
            FaceDetections[manager.MediaCaptureSettings.VideoDeviceId].Enabled = false;
        }

        #region Photo

        public event EventHandler<StorageFile> CapturePhotoComplete;

        public async Task<StorageFile> CapturePhoto(DeviceInformation camera = null, DeviceInformation microphone = null)
        {
            if (!Initialized)
                throw new Exception("Not initialized");
            if (CapturedVideo != null)
                throw new Exception("Already capturing");
            var format = ImageEncodingProperties.CreateJpeg();
            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(_SubFolderName, CreationCollisionOption.OpenIfExists);
            var name = $"{DateTime.Now.ToString("yymmddhhnnss")}.jpg";
            var file = await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
            var manager = await GetManagerAsync(camera, microphone);
            await manager.CapturePhotoToStorageFileAsync(format, file);
            CapturePhotoComplete?.Invoke(this, file);
            return file;
        }

        public async Task<IReadOnlyList<FileInformation>> ListPhotosAsync()
        {
            var options = new QueryOptions(CommonFileQuery.DefaultQuery, new[] { ".jpg" });
            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(_SubFolderName, CreationCollisionOption.OpenIfExists);
            var query = folder.CreateFileQueryWithOptions(options);
            var factory = new FileInformationFactory(query, Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            return await factory.GetFilesAsync();
        }

        #endregion

        #region Video

        StorageFile CapturedVideo;
        public event EventHandler CaptureVideoStarted;
        public event EventHandler<StorageFile> CaptureVideoStopped;

        public async Task StartCaptureVideo(DeviceInformation camera = null, DeviceInformation microphone = null)
        {
            if (!Initialized)
                throw new Exception("Not initialized");
            if (CapturedVideo != null)
                throw new InvalidOperationException("Already capturing");
            var profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
            var folder = await KnownFolders.VideosLibrary.CreateFolderAsync(_SubFolderName, CreationCollisionOption.OpenIfExists);
            var name = $"{DateTime.Now.ToString("yymmddhhnnss")}.mp4";
            CapturedVideo = await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
            var manager = await GetManagerAsync(camera, microphone);
            await manager.StartRecordToStorageFileAsync(profile, CapturedVideo);
            CaptureVideoStarted?.Invoke(this, EventArgs.Empty);
        }

        public async Task<StorageFile> StopCaptureVideoAsync(DeviceInformation camera = null, DeviceInformation microphone = null)
        {
            if (CapturedVideo == null)
                throw new InvalidOperationException("Not capturing");
            var manager = await GetManagerAsync(camera, microphone);
            await manager.StopRecordAsync();
            var result = CapturedVideo; CapturedVideo = null;
            CaptureVideoStopped?.Invoke(this, result);
            return result;
        }

        public async Task<IReadOnlyList<FileInformation>> ListVideosAsync()
        {
            var options = new QueryOptions(CommonFileQuery.DefaultQuery, new[] { ".mp4" });
            var folder = await KnownFolders.VideosLibrary.CreateFolderAsync(_SubFolderName, CreationCollisionOption.OpenIfExists);
            var query = folder.CreateFileQueryWithOptions(options);
            var factory = new FileInformationFactory(query, Windows.Storage.FileProperties.ThumbnailMode.VideosView);
            return await factory.GetFilesAsync();
        }

        #endregion  
    }
}
