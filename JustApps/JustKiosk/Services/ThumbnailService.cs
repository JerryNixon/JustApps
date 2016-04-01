using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;

namespace JustKiosk.Services
{
    public class ThumbnailService
    {
        public static ThumbnailService Instance;

        static ThumbnailService()
        {
            Instance = new ThumbnailService();
        }

        private ThumbnailService()
        {
            // nothing
        }

        StorageFolder _photoFolder;
        public async Task<StorageFolder> GetPhotoFolderAsync(string subfolder = "CameraService")
        {
            if (_photoFolder != null)
                return _photoFolder;
            _photoFolder = KnownFolders.PicturesLibrary;
            _photoFolder = await _photoFolder.CreateFolderAsync(subfolder, CreationCollisionOption.OpenIfExists);
            _photoFolder = await _photoFolder.CreateFolderAsync(DateTime.Now.ToString("yymm"), CreationCollisionOption.OpenIfExists);
            return _photoFolder;
        }

        public async Task<IReadOnlyList<FileInformation>> ListPhotosAsync()
        {
            var options = new QueryOptions(CommonFileQuery.DefaultQuery, new[] { ".jpg" });
            var query = (await GetPhotoFolderAsync()).CreateFileQueryWithOptions(options);
            var factory = new FileInformationFactory(query, Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            return await factory.GetFilesAsync();
        }

        StorageFolder _videoFolder;
        public async Task<StorageFolder> GetVideoFolderAsync(string subfolder = "CameraService")
        {
            if (_videoFolder != null)
                return _videoFolder;
            _videoFolder = KnownFolders.VideosLibrary;
            _videoFolder = await _videoFolder.CreateFolderAsync(subfolder, CreationCollisionOption.OpenIfExists);
            _videoFolder = await _videoFolder.CreateFolderAsync(DateTime.Now.ToString("yymm"), CreationCollisionOption.OpenIfExists);
            return _videoFolder;
        }

        public async Task<IReadOnlyList<FileInformation>> ListVideosAsync()
        {
            var options = new QueryOptions(CommonFileQuery.DefaultQuery, new[] { ".mp4" });
            var query = (await GetVideoFolderAsync()).CreateFileQueryWithOptions(options);
            var factory = new FileInformationFactory(query, Windows.Storage.FileProperties.ThumbnailMode.VideosView);
            return await factory.GetFilesAsync();
        }
    }
}
