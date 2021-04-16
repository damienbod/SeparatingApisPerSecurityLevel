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
        private readonly MyApiServiceTwoClient _myApiClientService;

        public DirectApiController(MyApiServiceTwoClient myApiClientService)
        {
            _myApiClientService = myApiClientService;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IEnumerable<string>> GetAsync()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            var data = await _myApiClientService.GetServiceTwoApiData();

            return new List<string> { "some data", "more data", "loads of data" };
        }
    }
}
