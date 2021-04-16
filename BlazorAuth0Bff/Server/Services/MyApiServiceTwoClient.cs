using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAuth0Bff.Server
{
    public class MyApiServiceTwoClient
    {
        private readonly IConfiguration _configurations;
        private readonly IHttpClientFactory _clientFactory;
        private readonly Auth0CCTokenApiService _auth0TokenApiService;

        public MyApiServiceTwoClient(
            IConfiguration configurations, 
            IHttpClientFactory clientFactory,
            Auth0CCTokenApiService auth0TokenApiService)
        {
            _configurations = configurations;
            _clientFactory = clientFactory;
            _auth0TokenApiService = auth0TokenApiService;
        }

        public async Task<JArray> GetServiceTwoApiData()
        {
            try
            {
                var client = _clientFactory.CreateClient();

                client.BaseAddress = new Uri(_configurations["MyApiUrl"]);

                var access_token = await _auth0TokenApiService.GetApiToken(client, "ServiceTwoApi");

                client.SetBearerToken(access_token);

                var response = await client.GetAsync("api/ServiceTwo");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
