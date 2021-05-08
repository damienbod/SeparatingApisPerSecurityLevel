using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyApi.Controllers
{
    /// <summary>
    /// User access token protected using Auth0 
    /// protected using "p-service-api-auth0" policy defined in the Startup
    /// </summary>
    [Authorize(Policy = "p-service-api-auth0")]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTwoController : ControllerBase
    {
        private readonly ILogger<UserOneController> _logger;

        public ServiceTwoController(ILogger<UserOneController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// returns data id the correct Auth0 access token is used.
        /// </summary>
        /// <returns>protected data</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IEnumerable<string> Get()
        {
            return new List<string> { "service two data" };
        }
    }
}
