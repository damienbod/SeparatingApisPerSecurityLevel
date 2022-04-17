using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MyApi.Controllers;

/// <summary>
/// User access token protected using Auth0 
/// protected using "p-user-api-auth0" policy defined in the Startup
/// </summary>
[SwaggerTag("User access token protected using Auth0")]
[Authorize(Policy = "p-user-api-auth0")]
[ApiController]
[Route("api/[controller]")]
public class UserOneController : ControllerBase
{
    /// <summary>
    /// returns data id the correct Auth0 access token is used.
    /// </summary>
    /// <returns>protected data</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IEnumerable<string> Get()
    {
        return new List<string> { "user one data" };
    }
}