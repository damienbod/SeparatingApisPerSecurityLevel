using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BlazorAuth0Bff.Server;

public class Auth0CCTokenApiService
{
    private readonly ILogger<Auth0CCTokenApiService> _logger;
    private readonly Auth0ApiConfiguration _auth0ApiConfiguration;

    private static readonly object _lock = new();
    private readonly IDistributedCache _cache;

    private const int cacheExpirationInDays = 1;

    private class AccessTokenResult
    {
        public string AcessToken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
    }

    private class AccessTokenItem
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }

    public Auth0CCTokenApiService(
            IOptions<Auth0ApiConfiguration> auth0ApiConfiguration,
            ILoggerFactory loggerFactory,
            IDistributedCache cache)
    {
        _auth0ApiConfiguration = auth0ApiConfiguration.Value;
        _logger = loggerFactory.CreateLogger<Auth0CCTokenApiService>();
        _cache = cache;
    }

    public async Task<string> GetApiToken(HttpClient client, string api_name)
    {
        var accessToken = GetFromCache(api_name);

        if (accessToken != null)
        {
            if (accessToken.ExpiresIn > DateTime.UtcNow)
            {
                return accessToken.AcessToken;
            }
            else
            {
                // remove  => NOT Needed for this cache type
            }
        }

        _logger.LogDebug("GetApiToken new from oauth server for {api_name}", api_name);

        // add
        var newAccessToken = await GetApiTokenClient(client);
        AddToCache(api_name, newAccessToken);

        return newAccessToken.AcessToken;
    }

    private async Task<AccessTokenResult> GetApiTokenClient(HttpClient client)
    {
        var payload = new Auth0ClientCrendentials
        {
            client_id = _auth0ApiConfiguration.ClientId,
            client_secret = _auth0ApiConfiguration.ClientSecret,
            audience = _auth0ApiConfiguration.Audience
        };

        var authUrl = _auth0ApiConfiguration.Url;
        var tokenResponse = await client.PostAsJsonAsync(authUrl, payload);

        if (tokenResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var result = await tokenResponse.Content.ReadFromJsonAsync<AccessTokenItem>();
            DateTime expirationTime = DateTimeOffset.FromUnixTimeSeconds(result.ExpiresIn).DateTime;
            return new AccessTokenResult
            {
                AcessToken = result.AccessToken,
                ExpiresIn = expirationTime
            };
        }

        _logger.LogError("tokenResponse.IsError Status code: {tokenResponse.StatusCode}, Error: {tokenResponse.ReasonPhrase}",
            tokenResponse.StatusCode, tokenResponse.ReasonPhrase);

        var errorMessage = await tokenResponse.Content.ReadAsStringAsync();
        _logger.LogError("{Error}", errorMessage);
        throw new ApplicationException($"Status code: {tokenResponse.StatusCode}, Error: {tokenResponse.ReasonPhrase}, message: {errorMessage}");
    }

    private void AddToCache(string key, AccessTokenResult accessTokenItem)
    {
        var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(cacheExpirationInDays));

        lock (_lock)
        {
            _cache.SetString(key, System.Text.Json.JsonSerializer.Serialize(accessTokenItem), options);
        }
    }

    private AccessTokenResult GetFromCache(string key)
    {
        var item = _cache.GetString(key);
        if (item != null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<AccessTokenResult>(item);
        }

        return null;
    }
}