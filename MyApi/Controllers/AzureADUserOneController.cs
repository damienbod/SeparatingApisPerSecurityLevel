using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace MyApi.Controllers;

/// <summary>
/// API protected with Microsoft.Identity.Web and Microsoft Entra ID
/// scope from App registration used to authorize.
/// </summary>
[SwaggerTag("API protected with Microsoft.Identity.Web and Microsoft Entra ID")]
[AuthorizeForScopes(Scopes = ["api://b2a09168-54e2-4bc4-af92-a710a64ef1fa/access_as_user"],
    AuthenticationScheme = "myADscheme")]
[Authorize(AuthenticationSchemes = "myADscheme")]
[ApiController]
[Route("api/[controller]")]
public class AzureADUserOneController : ControllerBase
{
    /// <summary>
    /// returns data id the correct Microsoft Entra ID access token is used with the correct scope.
    /// </summary>
    /// <returns>protected data</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IEnumerable<string> Get()
    {
        return new List<string> { "Microsoft Entra ID user one data" };
    }
}