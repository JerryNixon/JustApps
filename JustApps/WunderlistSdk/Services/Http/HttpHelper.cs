using Windows.Web.Http;
using System;
using System.Collections.Generic;
using Windows.Web.Http.Filters;

namespace WunderlistSdk.Service.Http
{

    public class HttpHelper
    {
        static HttpHelper()
        {
            _filter = new Lazy<HttpBaseProtocolFilter>(() =>
            {
                var filter = new HttpBaseProtocolFilter()
                {
                    MaxConnectionsPerServer = 15
                };
                filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default;
                filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;
                return filter;
            });

            _client = new Lazy<HttpClient>(() =>
            {
                return new HttpClient(_filter.Value);
            });
        }

        static Lazy<HttpClient> _client;
        public HttpClient Client => _client.Value;

        static Lazy<HttpBaseProtocolFilter> _filter;
        public HttpBaseProtocolFilter Filter => _filter.Value;

        public IEnumerable<KeyValuePair<string, string>> GetHeaders(HttpResponseMessage response)
        {
            foreach (var header in response.Headers) yield return header;
            foreach (var header in response.Content.Headers) yield return header;
        }
    }
}
