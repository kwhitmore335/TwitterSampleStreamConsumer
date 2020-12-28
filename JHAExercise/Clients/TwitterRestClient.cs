using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Text;
using System.Threading.Tasks;

namespace JHAExercise.Clients
{
    public class TwitterRestClient : ITwitterRestClient
    {
        private readonly RestClient _client;
        private readonly string _hostUrl;
        public string HostUrl => _hostUrl;

        public TwitterRestClient(IConfigurationRoot configuration)
        {
            var configSection = configuration.GetSection(typeof(TwitterRestClient).Name);
            _hostUrl = configSection.GetValue<string>("HostUrl");
            var key = configSection.GetValue<string>("Key");
            var secret = configSection.GetValue<string>("Secret");

            _client = new RestClient(HostUrl)
            {
                Authenticator = new TwitterBearerTokenAuthenticator(HostUrl, key, secret),
                Encoding = Encoding.Unicode,
                AutomaticDecompression = true
            };
        }

        public async Task<IRestResponse> ExecuteRequest(RestRequest restRequest)
        {            
            return await _client.ExecuteAsync(restRequest);
        }
    }
}
