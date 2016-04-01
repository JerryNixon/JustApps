using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;

namespace JustKiosk.Services.CameraService
{
    public class PreviewClass
    {
        FaceDetectionEffect FaceDetection => CameraService.FaceDetection;
        MediaCapture DefaultManager => CameraService.DefaultManager;

        public PreviewClass()
        {
            // nothing
        }

        public bool Previewing { get; private set; }

        CaptureElement PreviewControl;

        public async Task<VideoEncodingProperties> StartAsync(CaptureElement element)
        {
            if (DefaultManager == null)
                throw new Exception("Not initialized");
            if (Previewing)
                throw new Exception("Already previewing");
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            try
            {
                FaceDetection.Enabled = true;
                PreviewControl = element;
                PreviewControl.Source = DefaultManager;
                await DefaultManager.StartPreviewAsync();
                Previewing = true;
            }
            catch
            {
                throw;
            }

            var properties = DefaultManager.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            var videoEncodingProperties = properties as VideoEncodingProperties;
            return videoEncodingProperties;
        }

        public async Task StopAsync()
        {
            if (DefaultManager == null)
                throw new Exception("Not initialized");
            if (!Previewing)
                throw new Exception("Not previewing");
            try
            {
                FaceDetection.Enabled = false;
                await DefaultManager.StopPreviewAsync();
                Previewing = false;
            }
            catch
            {
                throw;
            }
        }
    }
}
