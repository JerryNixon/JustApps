using System.Threading.Tasks;
using Windows.Web.Http;
using System;
using Windows.Web.Http.Filters;
using System.Threading;
using System.Diagnostics;
using Template10.Services.SerializationService;

namespace WunderlistSdk.Service.Http
{
    public class RestService
    {
        RestHelper _helper;

        public RestService()
        {
            _helper = new RestHelper();
        }

        public async Task<T> GetAsync<T>(Uri uri) => await _helper.GetAsync<T>(uri);
        public async Task<HttpResponseMessage> PostAsync(Uri uri, object payload) => await _helper.PostAsync(uri, payload);
    }

    public partial class RestHelper
    {
        // https://msdn.microsoft.com/en-us/library/windows/apps/windows.web.http.httpclient.aspx

        static RestHelper()
        {
            _httpHelper = new HttpHelper();

            _Serializer = new Lazy<ISerializationService>(() =>
            {
                return SerializationService.Json;
            });
        }

        static HttpHelper _httpHelper;
        public HttpClient Client = _httpHelper.Client;
        public HttpBaseProtocolFilter Filter => _httpHelper.Filter;

        static Lazy<ISerializationService> _Serializer;
        public ISerializationService Serializer => _Serializer.Value;

        public async Task<T> GetAsync<T>(Uri uri, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
        {
            var json = await GetStringAsync(uri, token, callback);
            return Serializer.Deserialize<T>(json);
        }

        public async Task<string> GetStringAsync(Uri uri, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
        {
            token = token ?? new CancellationTokenSource();
            callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(GetAsync)}: {e.BytesReceived} bytes of {e.TotalBytesToReceive} bytes"); });

            return await Client.GetStringAsync(uri).AsTask(token.Token, callback);
        }

        public async Task<HttpResponseMessage> PutAsync(Uri uri, object payload, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
        {
            token = token ?? new CancellationTokenSource();
            callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(PutAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

            var json = Serializer.Serialize(payload);
            var content = new HttpStringContent(json, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
            return await Client.PutAsync(uri, content).AsTask(token.Token, callback);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri, object payload, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
        {
            token = token ?? new CancellationTokenSource();
            callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(PostAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

            var data = Serializer.Serialize(payload);
            var content = new HttpStringContent(data, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
            return await Client.PostAsync(uri, content);
        }

        public async Task<T> PostAsync<T>(Uri uri, object payload, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
        {
            token = token ?? new CancellationTokenSource();
            callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(PostAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

            var response = await PostAsync(uri, payload, token, callback);
            if (!response.IsSuccessStatusCode)
            {
                return default(T);
            }
            var responseJson = await response.Content.ReadAsStringAsync();
            return Serializer.Deserialize<T>(responseJson);
        }

        public async Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationTokenSource token = null, Progress<HttpProgress> callback = null)
        {
            token = token ?? new CancellationTokenSource();
            callback = callback ?? new Progress<HttpProgress>(e => { Debug.WriteLine($"{nameof(DeleteAsync)}: {e.BytesSent} bytes of {e.TotalBytesToSend} bytes"); });

            return await Client.DeleteAsync(uri).AsTask(token.Token, callback);
        }
    }
}
