using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuth0Bff.Server.Controllers
{
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class DirectApiController : ControllerBase
    {
        private readonly Auth0TokenApiService _auth0TokenApiService;
        private readonly IHttpClientFactory _clientFactory;

        public DirectApiController(Auth0TokenApiService auth0TokenApiService, IHttpClientFactory clientFactory)
        {
            _auth0TokenApiService = auth0TokenApiService;
            _clientFactory = clientFactory;
        }
        [HttpGet]
        public async System.Threading.Tasks.Task<IEnumerable<string>> GetAsync()
        {
            var client = _clientFactory.CreateClient();
            var data = await _auth0TokenApiService.GetApiToken(client, "https://auth0-api1");
            return new List<string> { "some data", "more data", "loads of data" };
        }
    }
}
