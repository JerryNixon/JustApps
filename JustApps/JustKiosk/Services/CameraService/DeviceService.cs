using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;

namespace JustKiosk.Services
{
    public class DeviceService
    {
        public static DeviceService Instance;

        static DeviceService()
        {
            Instance = new DeviceService();
        }

        private DeviceService()
        {
            // nothing
        }

        private bool Initialized { get; set; }

        public async Task InitializeAsync()
        {
            if (Initialized)
                return;
            Initialized = true;
            Cameras = await GetCamerasAsync(() =>
            {
                CamerasChanged?.Invoke(this, EventArgs.Empty);
            });
            Microphones = await GetMicrophonesAsync(() =>
            {
                MicrophonesChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        public DeviceInformation GetCamera(string id)
        {
            if (!Cameras.Any())
                return null;
            var camera = Cameras.FirstOrDefault(x => x.Id.ToLower() == id?.ToLower());
            if (camera == null)
                camera = Cameras.FirstOrDefault(x => x.EnclosureLocation?.Panel == Panel.Front);
            return camera ?? Cameras.First();
        }

        public DeviceInformationCollection Cameras { get; private set; }
        public event EventHandler CamerasChanged;

        DeviceWatcher VideoWatcher;
        Action VideoWatcherCallback;
        async Task<DeviceInformationCollection> GetCamerasAsync(Action callback)
        {
            if (callback != null)
            {
                VideoWatcherCallback = callback;
                if (VideoWatcher == null)
                {
                    VideoWatcher = DeviceInformation.CreateWatcher(DeviceClass.VideoCapture);
                    VideoWatcher.Added += (s, e) => VideoWatcherCallback();
                    VideoWatcher.Removed += (s, e) => VideoWatcherCallback();
                    VideoWatcher.Updated += (s, e) => VideoWatcherCallback();
                    VideoWatcher.Start();
                }
            }
            return await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        }

        public DeviceInformationCollection Microphones { get; private set; }
        public event EventHandler MicrophonesChanged;

        DeviceWatcher AudioWatcher;
        Action AudioWatcherCallback;
        async Task<DeviceInformationCollection> GetMicrophonesAsync(Action callback)
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
    }
}
