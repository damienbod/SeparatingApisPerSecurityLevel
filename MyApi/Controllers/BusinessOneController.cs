using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BusinessOneController : ControllerBase
    {
        private readonly ILogger<BusinessOneController> _logger;

        public BusinessOneController(ILogger<BusinessOneController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string> { "business one data" };
        }
    }
}
