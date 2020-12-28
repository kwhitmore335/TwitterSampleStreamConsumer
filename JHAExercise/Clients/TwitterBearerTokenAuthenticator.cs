using System;
using System.IO;
using System.Net;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;

namespace JHAExercise.Clients
{
    public class TwitterBearerTokenAuthenticator : IAuthenticator
    {
        private readonly string _key;
        private readonly string _secret;
        private readonly string _hostUrl;
        private string _token;        
        private string AuthUrl => $"{_hostUrl}/oauth2/token";

        public TwitterBearerTokenAuthenticator(string hostUrl, string key, string secret)
        {
            _hostUrl = hostUrl;
            _key = key;
            _secret = secret;
            _token = null;
        }

        #region IAuthenticator
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            if (string.IsNullOrEmpty(_token))
            {
                GetAuthToken(client.Proxy);
            }

            request.AddHeader("Authorization", $"Bearer {_token}");
        }
        #endregion

        private void GetAuthToken(IWebProxy proxy)
        {
            var authRequest = CreateAuthRequest(proxy);

            using var response = (HttpWebResponse)authRequest.GetResponse();
            var authResult = GetAuthResponse(response);
            var authToken = authResult?.access_token?.Value.ToString();
            if (string.IsNullOrEmpty(authToken))
            {
                throw new InvalidOperationException("Retrieved null or empty token");
            }
            _token = authToken;
        }
        private static dynamic GetAuthResponse(HttpWebResponse response)
        {
            using var data = response.GetResponseStream();
            if (data == null)
            {
                throw new InvalidOperationException("The auth response stream is null");
            }
            return GetObjectFromResponse<dynamic>(data);
        }
        private HttpWebRequest CreateAuthRequest(IWebProxy proxy)
        {
            var key = System.Web.HttpUtility.UrlEncode(_key);
            var secret = System.Web.HttpUtility.UrlEncode(_secret);
            var credentials = $"{key}:{secret}";
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var request = (HttpWebRequest)WebRequest.Create(new Uri(AuthUrl));
            request.Headers.Add("Authorization", $"Basic {encodedCredentials}");
            request.ProtocolVersion = HttpVersion.Version11;            
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.KeepAlive = false;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            request.Method = "POST";
            request.Accept = "application/json;";
            using (var stream = request.GetRequestStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write("grant_type=client_credentials");
                writer.Flush();
            }
            request.AutomaticDecompression = DecompressionMethods.None;

            return request;
        }
        private static T GetObjectFromResponse<T>(Stream data)
        {
            T result;
            using (var reader = new StreamReader(data))
            {
                var serialiser = new JsonSerializer();
                using var jsonTextReader = new JsonTextReader(reader);
                result = serialiser.Deserialize<T>(jsonTextReader);
            }
            return result;
        }
    }
}
