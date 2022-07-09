using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorAuth0Bff.Server;

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

    public async Task<List<string>> GetServiceTwoApiData()
    {
        var client = _clientFactory.CreateClient();

        client.BaseAddress = new Uri(_configurations["MyApiUrl"]);

        var access_token = await _auth0TokenApiService.GetApiToken(client, "ServiceTwoApi");

        client.DefaultRequestHeaders.Authorization 
            = new AuthenticationHeaderValue("Bearer", access_token);

        var response = await client.GetAsync("api/ServiceTwo");
        if (response.IsSuccessStatusCode)
        {
            var data = await JsonSerializer.DeserializeAsync<List<string>>(
            await response.Content.ReadAsStreamAsync());

            if(data != null)
                return data;
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}, message: {errorMessage}");
    }
}