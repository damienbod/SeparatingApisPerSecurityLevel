using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace MyApi.Controllers
{
    [Authorize(AuthenticationSchemes = "myADscheme")]
    [AuthorizeForScopes(Scopes = new string[] { "api://72286b8d-5010-4632-9cea-e69e565a5517/user_impersonation" }, AuthenticationScheme = "myADscheme")]
    [ApiController]
    [Route("api/[controller]")]
    public class AzureADUserOneController : ControllerBase
    {
        private readonly ILogger<UserOneController> _logger;

        public AzureADUserOneController(ILogger<UserOneController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string> { "AzureADUser one data" };
        }
    }
}
