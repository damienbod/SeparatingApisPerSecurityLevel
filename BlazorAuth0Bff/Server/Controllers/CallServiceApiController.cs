namespace BlazorAuth0Bff.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class CallServiceApiController : ControllerBase
{
    private readonly MyApiServiceTwoClient _myApiClientService;

    public CallServiceApiController(MyApiServiceTwoClient myApiClientService)
    {
        _myApiClientService = myApiClientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        // call service API
        var serviceData = await _myApiClientService.GetServiceTwoApiData();

        return Ok(serviceData);
    }
}