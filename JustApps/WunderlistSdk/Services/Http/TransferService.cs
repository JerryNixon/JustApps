using System.Threading.Tasks;
using Windows.Web.Http;
using System;
using System.Collections.Generic;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace WunderlistSdk.Service.Http
{
    public class TransferService : ITransferService
    {
        TransferHelper _helper;

        public TransferService()
        {
            _helper = new TransferHelper();
        }

        public async Task<IReadOnlyList<DownloadOperation>> GetDownloadQueueAsync() => await _helper.GetDownloadQueueAsync();
        public async Task<IReadOnlyList<UploadOperation>> GetUploadQueueAsync() => await _helper.GetUploadQueueAsync();
        public async Task<HttpResponseMessage> UploadAsync(Uri target, StorageFile file) => await _helper.UploadAsync(target, file);
        public async Task<HttpResponseMessage> DownloadAsync(Uri source, StorageFile file) => await _helper.DownloadAsync(source, file);
        public async Task<UploadOperation> QueueUploadAsync(Uri target, StorageFile file) => await _helper.QueueUploadAsync(target, file);
        public async Task<DownloadOperation> QueueDownloadAsync(Uri source, StorageFile file) => await _helper.QueueDownloadAsync(source, file);
    }
}
