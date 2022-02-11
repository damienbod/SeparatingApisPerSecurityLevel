using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Swashbuckle.AspNetCore.Annotations;

namespace MyApi.Controllers;

/// <summary>
/// API protected with Microsoft.Identity.Web and Azure AD
/// scope from App registration used to authorize.
/// </summary>
[SwaggerTag("API protected with Microsoft.Identity.Web and Azure AD")]
[AuthorizeForScopes(Scopes = new string[] { "api://b2a09168-54e2-4bc4-af92-a710a64ef1fa/access_as_user" },
    AuthenticationScheme = "myADscheme")]
[Authorize(AuthenticationSchemes = "myADscheme")]
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