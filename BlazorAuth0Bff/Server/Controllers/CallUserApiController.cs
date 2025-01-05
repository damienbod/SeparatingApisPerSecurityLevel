namespace BlazorAuth0Bff.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class CallUserApiController : ControllerBase
{
    private readonly MyApiUserOneClient _myApiUserOneClient;

    public CallUserApiController(MyApiUserOneClient myApiUserOneClient)
    {
        _myApiUserOneClient = myApiUserOneClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        // call user API
        string? accessToken = await HttpContext.GetTokenAsync("access_token");
        if (accessToken != null)
        {
            var userData = await _myApiUserOneClient.GetUserOneApiData(accessToken);

            return Ok(userData);
        }

        return Unauthorized("no access token");
    }
}