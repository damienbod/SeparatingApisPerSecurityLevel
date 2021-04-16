using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAuth0Bff.Server
{
    /// <summary>
    /// setup to oidc client in the startup correctly
    /// https://auth0.com/docs/quickstart/webapp/aspnet-core#enterprise-saml-and-others-
    /// </summary>
    public class MyApiUserOneClient
    {
        private readonly IConfiguration _configurations;
        private readonly IHttpClientFactory _clientFactory;

        public MyApiUserOneClient(
            IConfiguration configurations, 
            IHttpClientFactory clientFactory)
        {
            _configurations = configurations;
            _clientFactory = clientFactory;
        }

        public async Task<JArray> GetUserOneApiData(string accessToken)
        {
            try
            {
                var client = _clientFactory.CreateClient();

                client.BaseAddress = new Uri(_configurations["MyApiUrl"]);

                client.SetBearerToken(accessToken);

                var response = await client.GetAsync("api/UserOne");
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
