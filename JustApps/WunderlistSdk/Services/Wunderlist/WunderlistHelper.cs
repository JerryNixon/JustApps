using System;
using Template10.Utils;
using Windows.Security.Authentication.Web;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;
using Template10.Services.NetworkAvailableService;
using System.Collections.Generic;
using System.Linq;
using WunderlistSdk.Service.Http;

namespace WunderlistSdk.Wunderlist
{
    internal class WunderlistHelper
    {
        private static NetworkAvailableService _NetworkService = new NetworkAvailableService();
        private static SecretService _SecretService = new SecretService();
        private static RestHelper _RestHelper = new RestHelper();
        private WunderlistSettings _settings;

        public WunderlistHelper(WunderlistSettings settings)
        {
            _settings = settings;
        }

        string Token
        {
            get { return _SecretService.ReadSecret(GetType().ToString(), nameof(Token)); }
            set { _SecretService.WriteSecret(GetType().ToString(), nameof(Token), value); }
        }

        bool IsConnected => _NetworkService.IsInternetAvailable().Result;

        public Models.User User { get; private set; }

        public async Task<bool> AuthenticateSilentlyAsync()
        {
            if (!IsConnected)
            {
                return false;
            }

            if (string.IsNullOrEmpty(Token))
            {
                return false;
            }

            try
            {
                SetupAuthHeaders();
                User = await GetUserAsync();
                return true;
            }
            catch
            {
                Token = string.Empty;
                return false;
            }
        }

        public async Task<bool> AuthenticateAsync()
        {
            // https://developer.wunderlist.com/documentation/concepts/authorization

            if (!IsConnected)
            {
                return false;
            }

            if (await AuthenticateSilentlyAsync())
            {
                return true;
            }

            var code = await GetCodeFromUserAsync();
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            var token = Token = await GetTokenFromCodeAsync(code);
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            SetupAuthHeaders();
            User = await GetUserAsync();

            return true;
        }

        public async Task<IReadOnlyList<Models.Task>> GetTasksAsync(Models.List list)
        {
            var requestObject = new
            {
                list_id = list.Id,
            };
            var requestJson = JsonConvert.SerializeObject(requestObject);
            var requestContent = new HttpStringContent(requestJson, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
            var requestUri = new Uri("http://a.wunderlist.com/api/v1/lists");
            var response = await _RestHelper.PostAsync(requestUri, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var array = Newtonsoft.Json.Linq.JArray.Parse(json);
            var all = array.ToObject<Json.Task[]>();
            return all.Select(x => new Models.Task(x)).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<Models.List>> GetListsAsync()
        {
            var uri = new Uri("http://a.wunderlist.com/api/v1/lists");
            var json = await _RestHelper.GetStringAsync(uri);
            var array = Newtonsoft.Json.Linq.JArray.Parse(json);
            var all = array.ToObject<Json.List[]>();
            return all.Select(x => new Models.List(x)).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<Models.List>> GetListsAsync(Models.Folder folder)
        {
            var uri = new Uri("http://a.wunderlist.com/api/v1/lists");
            var json = await _RestHelper.GetStringAsync(uri);
            var array = Newtonsoft.Json.Linq.JArray.Parse(json);
            var all = array.ToObject<Json.List[]>();
            var filter = all.Where(x => folder.Lists.Contains(x.id));
            return filter.Select(x => new Models.List(x)).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<Models.Folder>> GetFoldersAsync()
        {
            var uri = new Uri("http://a.wunderlist.com/api/v1/folders");
            var json = await _RestHelper.GetStringAsync(uri);
            var array = Newtonsoft.Json.Linq.JArray.Parse(json);
            var list = array.ToObject<Json.Folder[]>();
            return list.Select(x => new Models.Folder(x)).ToList().AsReadOnly();
        }

        public async Task<Models.User> GetUserAsync()
        {
            var uri = new Uri("http://a.wunderlist.com/api/v1/user");
            var json = await _RestHelper.GetStringAsync(uri);
            var user = JsonConvert.DeserializeObject<Json.User>(json);
            return new Models.User(user);
        }

        #region private methods

        private async Task<string> GetCodeFromUserAsync()
        {
            var state = Uri.EscapeUriString(Guid.NewGuid().ToString());
            var client_id = Uri.EscapeUriString(_settings.CLIENT_ID);
            var redirect_uri = Uri.EscapeUriString(_settings.AUTH_CALLBACK_URL);
            var getCodeUri = new Uri($"https://www.wunderlist.com/oauth/authorize?client_id={client_id}&redirect_uri={redirect_uri}&state={state}");
            var callbackUri = new Uri(_settings.AUTH_CALLBACK_URL);

            WebAuthenticationResult response = null;
            try
            {
                response = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, getCodeUri, callbackUri);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            switch (response.ResponseStatus)
            {
                case WebAuthenticationStatus.UserCancel:
                case WebAuthenticationStatus.ErrorHttp:
                    return string.Empty;
            }

            var responseData = response.ResponseData.ToString();
            var responseQuerystring = new Uri(responseData).QueryString();
            var correlationState = responseQuerystring.GetFirstValueByName("state");
            if (correlationState != state)
            {
                return string.Empty;
            }

            return responseQuerystring.GetFirstValueByName("code");
        }

        private async Task<string> GetTokenFromCodeAsync(string code)
        {
            var requestObject = new
            {
                client_id = _settings.CLIENT_ID,
                client_secret = _settings.CLIENT_SECRET,
                code = code,
            };
            var requestJson = JsonConvert.SerializeObject(requestObject);
            var requestContent = new HttpStringContent(requestJson,
                Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

            var requestUri = new Uri($"https://www.wunderlist.com/oauth/access_token");
            var response = await _RestHelper.PostAsync(requestUri, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty; // <-- always a 422
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            dynamic responseModel = JsonConvert.DeserializeObject(responseJson);
            return responseModel.access_token;
        }

        private void SetupAuthHeaders()
        {
            var httpClientHeaders = _RestHelper.Client.DefaultRequestHeaders;

            var clientKey = "X-Client-ID";
            if (httpClientHeaders.ContainsKey(clientKey)) httpClientHeaders[clientKey] = _settings.CLIENT_ID;
            else httpClientHeaders.Add(clientKey, _settings.CLIENT_ID);

            var tokenKey = "X-Access-Token";
            if (httpClientHeaders.ContainsKey(tokenKey)) httpClientHeaders[tokenKey] = Token;
            else httpClientHeaders.Add(tokenKey, Token);
        }

        #endregion
    }
}
