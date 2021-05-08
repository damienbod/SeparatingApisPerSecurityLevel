using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace MyApi.Controllers
{
    /// <summary>
    /// API protected with Microsoft.Identity.Web and Azure AD
    /// scope from App registration used to authorize.
    /// </summary>
    [Authorize(AuthenticationSchemes = "myADscheme")]
    [AuthorizeForScopes(Scopes = new string[] { "api://72286b8d-5010-4632-9cea-e69e565a5517/user_impersonation" }, 
        AuthenticationScheme = "myADscheme")]
    [ApiController]
    [Route("api/[controller]")]
    public class AzureADUserOneController : ControllerBase
    {
        private readonly ILogger<UserOneController> _logger;

        public AzureADUserOneController(ILogger<UserOneController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// returns data id the correct Azure AD access token is used with the correct scope.
        /// </summary>
        /// <returns>protected data</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IEnumerable<string> Get()
        {
            return new List<string> { "AzureADUser one data" };
        }
    }
}
