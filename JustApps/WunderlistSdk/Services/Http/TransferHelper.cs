using System.Threading.Tasks;
using Windows.Web.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Networking.BackgroundTransfer;
using System.Diagnostics;
using Windows.Storage;
using Windows.Security.Credentials;
using Windows.UI.Notifications;
using Windows.Web.Http.Filters;

namespace WunderlistSdk.Service.Http
{
    public class TransferHelper
    {
        // https://msdn.microsoft.com/en-us/windows/uwp/networking/background-transfers
        // https://msdn.microsoft.com/en-us/library/windows/apps/hh943065.aspx?f=255&MSPPError=-2147217396

        static TransferHelper()
        {
            _httpHelper = new HttpHelper();
        }

        public enum UploadMethods { POST, PUT }

        static HttpHelper _httpHelper;

        public HttpClient Client = _httpHelper.Client;

        public HttpBaseProtocolFilter Filter => _httpHelper.Filter;

        public BackgroundTransferCompletionGroup BackgroundTransferCompletionGroup { get; }

        public BackgroundTransferGroup BackgroundTransferGroup { get; }

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        internal TransferHelper()
        {
            BackgroundTransferGroup = BackgroundTransferGroup.CreateGroup(nameof(TransferHelper));
            BackgroundTransferCompletionGroup = new BackgroundTransferCompletionGroup { };
        }

        public async Task<IReadOnlyList<DownloadOperation>> GetDownloadQueueAsync()
            => await BackgroundDownloader.GetCurrentDownloadsForTransferGroupAsync(BackgroundTransferGroup);

        public async Task<IReadOnlyList<UploadOperation>> GetUploadQueueAsync()
            => await BackgroundUploader.GetCurrentUploadsForTransferGroupAsync(BackgroundTransferGroup);

        public async Task<HttpResponseMessage> UploadAsync(Uri target, StorageFile file, 
            HttpMethod method = null, CancellationTokenSource token = null, IProgress<HttpProgress> callback = null)
        {
            token = token ?? new CancellationTokenSource(DefaultTimeout);
            callback = callback ?? new Progress<HttpProgress>(e =>
                { Debug.WriteLine($"{nameof(UploadAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });
            method = method ?? HttpMethod.Post;

            var multipart = new HttpMultipartFormDataContent();
            using (var stream = await file.OpenSequentialReadAsync())
            {
                var content = new HttpStreamContent(stream);
                multipart.Add(content, nameof(StorageFile), file.Name);
                var request = new HttpRequestMessage(method, target)
                {
                    Content = multipart
                };
                using (request)
                {
                    return await Client.SendRequestAsync(request).AsTask(token.Token, callback);
                }
            }
        }

        public async Task<HttpResponseMessage> DownloadAsync(Uri source, StorageFile file, 
            CancellationTokenSource token = null, IProgress<HttpProgress> callback = null)
        {
            token = token ?? new CancellationTokenSource(DefaultTimeout);
            callback = callback ?? new Progress<HttpProgress>(e =>
                { Debug.WriteLine($"{nameof(DownloadAsync)}: {e.BytesReceived} bytes of {e.TotalBytesToReceive} bytes"); });

            using (var response = await Client.GetAsync(source).AsTask(token.Token, callback))
            {
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                {
                    return response;
                }
                var buffer = await response.Content.ReadAsBufferAsync();
                await FileIO.WriteBufferAsync(file, buffer);
                return response;
            }
        }

        public async Task<UploadOperation> QueueUploadAsync(Uri target, StorageFile file, UploadMethods method = UploadMethods.PUT,
            PasswordCredential serverCredential = null, PasswordCredential proxyCredential = null,
            BackgroundTransferCostPolicy policy = BackgroundTransferCostPolicy.Default,
            BackgroundTransferPriority priority = BackgroundTransferPriority.Default,
            ToastNotification successToastNotification = null, ToastNotification failureToastNotification = null,
            TileNotification successTileNotification = null, TileNotification failureTileNotification = null,
            CancellationTokenSource token = null, IProgress<UploadOperation> callback = null)
        {
            token = token ?? new CancellationTokenSource(DefaultTimeout);
            callback = callback ?? new Progress<UploadOperation>(e => 
                { Debug.WriteLine($"{nameof(QueueUploadAsync)}: {e.Progress.BytesReceived} bytes of {e.Progress.TotalBytesToReceive} bytes"); });

            var uploader = new BackgroundUploader(BackgroundTransferCompletionGroup)
            {
                ServerCredential = serverCredential,
                ProxyCredential = proxyCredential,
                Method = method.ToString(),
                TransferGroup = BackgroundTransferGroup,
                CostPolicy = policy,
                SuccessToastNotification = successToastNotification,
                FailureToastNotification = failureToastNotification,
                SuccessTileNotification = successTileNotification,
                FailureTileNotification = failureTileNotification,
            };
            uploader.SetRequestHeader("Filename", file.Name);
            return await uploader.CreateUpload(target, file).StartAsync().AsTask(token.Token, callback);
        }

        public async Task<DownloadOperation> QueueDownloadAsync(Uri source, StorageFile file,
            PasswordCredential serverCredential = null, PasswordCredential proxyCredential = null,
            BackgroundTransferCostPolicy policy = BackgroundTransferCostPolicy.Default,
            BackgroundTransferPriority priority = BackgroundTransferPriority.Default,
            ToastNotification successToastNotification = null, ToastNotification failureToastNotification = null,
            TileNotification successTileNotification = null, TileNotification failureTileNotification = null,
            CancellationTokenSource token = null, IProgress<DownloadOperation> callback = null)
        {
            token = token ?? new CancellationTokenSource(DefaultTimeout);
            callback = callback ?? new Progress<DownloadOperation>(e => 
                { Debug.WriteLine($"{nameof(QueueDownloadAsync)}: {e.Progress.BytesReceived} bytes of {e.Progress.TotalBytesToReceive} bytes"); });

            var downloader = new BackgroundDownloader(BackgroundTransferCompletionGroup)
            {
                ServerCredential = serverCredential,
                ProxyCredential = proxyCredential,
                TransferGroup = BackgroundTransferGroup,
                CostPolicy = policy,
                SuccessToastNotification = successToastNotification,
                FailureToastNotification = failureToastNotification,
                SuccessTileNotification = successTileNotification,
                FailureTileNotification = failureTileNotification,
            };
            return await downloader.CreateDownload(source, file).StartAsync().AsTask(token.Token, callback);
        }

        public ToastNotification SimpleToastNotification(string text = "Background operation completed")
        {
            var xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var nodes = xml.GetElementsByTagName("text");
            nodes.Item(0).InnerText = text;
            return new ToastNotification(xml);
        }

        public TileNotification SimpleTileNotification(string text = "Background operation completed")
        {
            var xml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            var nodes = xml.GetElementsByTagName("text");
            nodes.Item(0).InnerText = text;
            return new TileNotification(xml);
        }
    }
}
