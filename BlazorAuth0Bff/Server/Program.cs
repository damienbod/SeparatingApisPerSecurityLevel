using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

services.AddAntiforgery(options =>
{
    options.HeaderName = AntiforgeryDefaults.HeaderName;
    options.Cookie.Name = AntiforgeryDefaults.CookieName;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

services.Configure<Auth0ApiConfiguration>(configuration.GetSection("Auth0ApiConfiguration"));
services.AddScoped<Auth0CCTokenApiService>();
services.AddScoped<MyApiServiceTwoClient>();
services.AddScoped<MyApiUserOneClient>();

services.AddHttpClient();
services.AddOptions();

// Add authentication services
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "__Host-BlazorServer";
    options.Cookie.SameSite = SameSiteMode.Lax;
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = $"https://{configuration["Auth0:Domain"]}";
    options.ClientId = configuration["Auth0:ClientId"];
    options.ClientSecret = configuration["Auth0:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("auth0-user-api-one");
    options.CallbackPath = new PathString("/signin-oidc");
    options.ClaimsIssuer = "Auth0";
    options.SaveTokens = true;
    options.UsePkce = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters.NameClaimType = "name";

    options.Events = new OpenIdConnectEvents
    {
        // handle the logout redirection 
        OnRedirectToIdentityProviderForSignOut = (context) =>
        {
            var logoutUri = $"https://{configuration["Auth0:Domain"]}/v2/logout?client_id={configuration["Auth0:ClientId"]}";

            var postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
                if (postLogoutUri.StartsWith("/"))
                {
                    // transform to absolute
                    var request = context.Request;
                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                }
                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
            }

            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
        },
        OnRedirectToIdentityProvider = context =>
        {
            // The context's ProtocolMessage can be used to pass along additional query parameters
            // to Auth0's /authorize endpoint.
            // 
            // Set the audience query parameter to the API identifier to ensure the returned Access Tokens can be used
            // to call protected endpoints on the corresponding API.
            context.ProtocolMessage.SetParameter("audience", "https://auth0-api1");

            return Task.FromResult(0);
        }
    };
});

services.AddControllersWithViews(options =>
     options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

var idp = $"https://{configuration["Auth0:Domain"]}";
app.UseSecurityHeaders(
    SecurityHeadersDefinitions
        .GetHeaderPolicyCollection(env.IsDevelopment(), idp));

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseNoUnauthorizedRedirect("/api");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapNotFound("/api/{**segment}");
app.MapFallbackToPage("/_Host");

app.Run();
