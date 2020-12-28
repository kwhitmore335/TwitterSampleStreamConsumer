using System;
using RestSharp;
using System.Threading.Tasks;

namespace JHAExercise.Clients
{
    public interface ITwitterRestClient
    {
        string HostUrl { get; }
        Task<IRestResponse> ExecuteRequest(RestRequest restRequest);
    }
}
