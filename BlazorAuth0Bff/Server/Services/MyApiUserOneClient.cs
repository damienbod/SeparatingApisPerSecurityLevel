using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
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

        public async Task<List<string>> GetUserOneApiData(string accessToken)
        {
            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri(_configurations["MyApiUrl"]);

            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("api/UserOne");
            if (response.IsSuccessStatusCode)
            {
                var data = await JsonSerializer.DeserializeAsync<List<string>>(
                await response.Content.ReadAsStreamAsync());

                return data;
            }

            var errorMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}, message: {errorMessage}");
        }
    }
}
