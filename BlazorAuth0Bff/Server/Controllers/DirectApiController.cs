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
        private readonly MyApiUserOneClient _myApiUserOneClient;

        public DirectApiController(MyApiServiceTwoClient myApiClientService,
            MyApiUserOneClient myApiUserOneClient)
        {
            _myApiClientService = myApiClientService;
            _myApiUserOneClient = myApiUserOneClient;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IEnumerable<string>> GetAsync()
        {
            // call service API
            var serviceData = await _myApiClientService.GetServiceTwoApiData();

            // call user API
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var userData = await _myApiUserOneClient.GetUserOneApiData(accessToken);
            return new List<string> { "some data", "more data", "loads of data" };
        }
    }
}
