using System.Threading.Tasks;
using Windows.Web.Http;
using System;
using System.Collections.Generic;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace WunderlistSdk.Service.Http
{
    public interface ITransferService
    {
        Task<HttpResponseMessage> DownloadAsync(Uri source, StorageFile file);
        Task<IReadOnlyList<DownloadOperation>> GetDownloadQueueAsync();
        Task<IReadOnlyList<UploadOperation>> GetUploadQueueAsync();
        Task<DownloadOperation> QueueDownloadAsync(Uri source, StorageFile file);
        Task<UploadOperation> QueueUploadAsync(Uri target, StorageFile file);
        Task<HttpResponseMessage> UploadAsync(Uri target, StorageFile file);
    }
}
