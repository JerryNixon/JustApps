using System.Threading.Tasks;
using Windows.Web.Http;
using System;
using Windows.Web.Http.Headers;
using Windows.Web.Http.Filters;
using System.Collections.Generic;
using System.Threading;
using Windows.Networking.BackgroundTransfer;
using System.Diagnostics;
using Windows.Storage;
using Windows.Security.Credentials;
using Windows.UI.Notifications;
using Template10.Services.SerializationService;

namespace JustPomodoro.Services
{
    public class HttpHelper
    {
        // https://msdn.microsoft.com/en-us/library/windows/apps/windows.web.http.httpclient.aspx

        static HttpHelper()
        {
            _filter = new HttpBaseProtocolFilter()
            {
                MaxConnectionsPerServer = 15
            };
            _filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default;
            _filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;

            _client = new HttpClient(_filter);

            var httpMediaTypeWithQualityHeaderValue = new HttpMediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(httpMediaTypeWithQualityHeaderValue);
        }

        public HttpHelper() : this(Template10.Services.SerializationService.SerializationService.Json)
        {
            // empty
        }

        public HttpHelper(ISerializationService serializationService)
        {
            _SerializationService = serializationService;
        }

        static HttpClient _client;
        public HttpClient Client => _client;

        static HttpBaseProtocolFilter _filter;
        public HttpBaseProtocolFilter Filter => _filter;

        static ISerializationService _SerializationService;

        public RestLogic Rest = new RestLogic();
        public class RestLogic
        {
            internal RestLogic() { }

            public async Task<T> GetAsync<T>(Uri uri, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
            {
                var json = await GetStringAsync(uri, token, callback);
                return _SerializationService.Deserialize<T>(json);
            }

            public async Task<string> GetStringAsync(Uri uri, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
            {
                token = token ?? new CancellationTokenSource();
                callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(GetAsync)}: {e.BytesReceived} bytes of {e.TotalBytesToReceive} bytes"); });

                return await _client.GetStringAsync(uri).AsTask(token.Token, callback);
            }

            public async Task<HttpResponseMessage> PutAsync(Uri uri, object payload, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
            {
                token = token ?? new CancellationTokenSource();
                callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(PutAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

                var json = _SerializationService.Serialize(payload);
                var content = new HttpStringContent(json, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                return await _client.PutAsync(uri, content).AsTask(token.Token, callback);
            }

            public async Task<HttpResponseMessage> PostAsync(Uri uri, object payload, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
            {
                token = token ?? new CancellationTokenSource();
                callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(PostAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

                var data = _SerializationService.Serialize(payload);
                var content = new HttpStringContent(data, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                return await _client.PostAsync(uri, content);
            }

            public async Task<T> PostAsync<T>(Uri uri, object payload, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
            {
                token = token ?? new CancellationTokenSource();
                callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(PostAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

                var requestJson = _SerializationService.Serialize(payload);
                var requestContent = new HttpStringContent(requestJson, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                var response = await _client.PostAsync(uri, requestContent);
                if (!response.IsSuccessStatusCode)
                {
                    return default(T);
                }
                var responseJson = await response.Content.ReadAsStringAsync();
                return _SerializationService.Deserialize<T>(responseJson);
            }


            public async Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
            {
                token = token ?? new CancellationTokenSource();
                callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(DeleteAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

                return await _client.DeleteAsync(uri).AsTask(token.Token, callback);

            }
        }

        public TransferLogic Transfer = new TransferLogic();
        public class TransferLogic
        {
            // https://msdn.microsoft.com/en-us/windows/uwp/networking/background-transfers
            // https://msdn.microsoft.com/en-us/library/windows/apps/hh943065.aspx?f=255&MSPPError=-2147217396

            public BackgroundTransferCompletionGroup BackgroundTransferCompletionGroup { get; private set; }

            public BackgroundTransferGroup BackgroundTransferGroup { get; private set; }

            internal TransferLogic()
            {
                BackgroundTransferGroup = BackgroundTransferGroup.CreateGroup(nameof(TransferLogic));
                BackgroundTransferCompletionGroup = new BackgroundTransferCompletionGroup { };
            }

            public async Task<IReadOnlyList<DownloadOperation>> GetDownloadQueueAsync()
            {
                return await BackgroundDownloader.GetCurrentDownloadsForTransferGroupAsync(BackgroundTransferGroup);
            }

            public async Task<IReadOnlyList<UploadOperation>> GetUploadQueueAsync()
            {
                return await BackgroundUploader.GetCurrentUploadsForTransferGroupAsync(BackgroundTransferGroup);
            }

            public enum UploadMethods { POST, PUT }

            public async Task<HttpResponseMessage> UploadAsync(Uri target, StorageFile source, HttpMethod method = null, CancellationTokenSource token = null)
            {
                token = token ?? new CancellationTokenSource();
                method = method ?? HttpMethod.Post;

                var multipart = new HttpMultipartFormDataContent();
                using (var stream = await source.OpenSequentialReadAsync())
                {
                    var content = new HttpStreamContent(stream);
                    multipart.Add(content, nameof(StorageFile), source.Name);
                    var request = new HttpRequestMessage(HttpMethod.Post, target)
                    {
                        Content = multipart
                    };
                    using (request)
                    {
                        return await _client.SendRequestAsync(request).AsTask(token.Token);
                    }
                }
            }

            public async Task<HttpResponseMessage> DownloadAsync(Uri source, StorageFile target, CancellationTokenSource token = null)
            {
                token = token ?? new CancellationTokenSource();

                using (var response = await _client.GetAsync(source).AsTask(token.Token))
                {
                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                    var buffer = await response.Content.ReadAsBufferAsync();
                    await FileIO.WriteBufferAsync(target, buffer);
                    return response;
                }
            }

            public async Task QueueUploadAsync(Uri destination, StorageFile source, UploadMethods method = UploadMethods.PUT, PasswordCredential serverCredential = null, PasswordCredential proxyCredential = null, BackgroundTransferCostPolicy policy = BackgroundTransferCostPolicy.Default, BackgroundTransferPriority priority = BackgroundTransferPriority.Default, ToastNotification successToastNotification = null, ToastNotification failureToastNotification = null, TileNotification successTileNotification = null, TileNotification failureTileNotification = null, CancellationTokenSource token = null, IProgress<UploadOperation> callback = null)
            {
                token = token ?? new CancellationTokenSource();
                callback = callback ?? new Progress<UploadOperation>(e => { Debug.WriteLine($"{nameof(QueueUploadAsync)}: {e.Progress.BytesReceived} bytes of {e.Progress.TotalBytesToReceive} bytes"); });

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
                uploader.SetRequestHeader("Filename", source.Name);
                await uploader.CreateUpload(destination, source).StartAsync().AsTask(token.Token, callback);
            }
        }


        public UtilityLogic Utility = new UtilityLogic();
        public class UtilityLogic
        {
            internal UtilityLogic() { }

            public void SetCookie(string name, string domain, string path, string value, DateTimeOffset? expires = null, bool httpOnly = false, bool secure = false)
            {
                var manager = _filter.CookieManager;
                var cookie = new HttpCookie(name, domain, path)
                {
                    Value = value,
                    Expires = expires,
                    HttpOnly = httpOnly,
                    Secure = secure
                };
                manager.SetCookie(cookie);
            }

            private HttpCookieCollection GetCookies(Uri uri)
            {
                var manager = _filter.CookieManager;
                return manager.GetCookies(uri);
            }

            public IEnumerable<KeyValuePair<string, string>> ExtractHeaders(HttpResponseMessage response)
            {
                foreach (var header in response.Headers) yield return header;
                foreach (var header in response.Content.Headers) yield return header;
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
}
