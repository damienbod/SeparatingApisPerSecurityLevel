using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyApi.Controllers
{
    [Authorize(Policy = "p-user-api-auth0")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserOneController : ControllerBase
    {
        private readonly ILogger<UserOneController> _logger;

        public UserOneController(ILogger<UserOneController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string> { "user one data" };
        }
    }
}
