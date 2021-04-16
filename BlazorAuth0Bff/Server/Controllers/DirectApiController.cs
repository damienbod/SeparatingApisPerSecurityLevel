﻿using System.Collections.Generic;
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
            var data = await _myApiClientService.GetServiceTwoApiData();

            string accessToken = await HttpContext.GetTokenAsync("access_token");
           // var userdata = await _myApiUserOneClient.GetUserOneApiData(accessToken);
            return new List<string> { "some data", "more data", "loads of data" };
        }
    }
}
