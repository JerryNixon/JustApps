using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace JustKiosk.Services.CameraService
{
    public class VideoClass
    {
        MediaCapture DefaultManager => CameraService.DefaultManager;

        public VideoClass()
        {
            // nothing
        }

        StorageFile CaptureVideoFile;
        public bool Capturing { get; private set; }
        public event EventHandler CaptureVideoStarted;
        public event EventHandler<StorageFile> CaptureVideoStopped;

        public async Task StartCaptureVideo(string subfolder = "CameraService")
        {
            if (DefaultManager == null)
                throw new Exception("Not initialized");
            if (Capturing)
                throw new InvalidOperationException("Already capturing");
            var profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
            var folder = await KnownFolders.VideosLibrary.CreateFolderAsync(subfolder, CreationCollisionOption.OpenIfExists);
            var name = $"{DateTime.Now.ToString("yymmddhhnnss")}.mp4";
            try
            {
                CaptureVideoFile = await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
                await DefaultManager.StartRecordToStorageFileAsync(profile, CaptureVideoFile);
                Capturing = true;
            }
            catch
            {
                throw;
            }
            CaptureVideoStarted?.Invoke(this, EventArgs.Empty);
        }

        public async Task<StorageFile> StopAsync()
        {
            if (!Capturing)
                throw new InvalidOperationException("Not capturing");
            var file = CaptureVideoFile;
            try
            {
                await DefaultManager.StopRecordAsync();
                CaptureVideoStopped?.Invoke(this, file);
                Capturing = false;
                return CaptureVideoFile;
            }
            catch
            {
                throw;
            }
            finally
            {
                CaptureVideoFile = null;
            }
        }
    }
}
