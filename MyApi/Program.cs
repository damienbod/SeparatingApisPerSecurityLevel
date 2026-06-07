using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi;
using MyApi;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddSecurityHeaderPolicies()
  .SetPolicySelector((PolicySelectorContext ctx) =>
  {
      return SecurityHeadersDefinitions.GetHeaderPolicyCollection(
          builder.Environment.IsDevelopment());
  });

// only needed for browser clients
//services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllOrigins",
//        builder =>
//        {
//            builder
//                .AllowCredentials()
//                .WithOrigins(
//                    "https://localhost:4200")
//                .SetIsOriginAllowedToAllowWildcardSubdomains()
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//        });
//});

// Adds Microsoft Identity platform (AAD v2.0) support to protect this Api
services.AddMicrosoftIdentityWebApiAuthentication(configuration, "AzureAd", "myADscheme");

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-damienbod.eu.auth0.com/";
    options.Audience = "https://auth0-api1";
});

services.AddOpenApi(options =>
{
    //options.UseTransformer((document, context, cancellationToken) =>
    //{
    //    document.Info = new()
    //    {
    //        Title = "My API",
    //        Version = "v1",
    //        Description = "API for Damien"
    //    };
    //    return Task.CompletedTask;
    //});
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

services.AddSingleton<IAuthorizationHandler, UserApiScopeHandler>();

services.AddAuthorization(policies =>
{
    policies.AddPolicy("p-user-api-auth0", p =>
    {
        p.Requirements.Add(new UserApiScopeHandlerRequirement());
        // Validate id of application for which the token was created
        p.RequireClaim("azp", "AScjLo16UadTQRIt2Zm1xLHVaEaE1feA");
    });

    policies.AddPolicy("p-service-api-auth0", p =>
    {
        // Validate id of application for which the token was created
        p.RequireClaim("azp", "naWWz6gdxtbQ68Hd2oAehABmmGM9m1zJ");
        p.RequireClaim("gty", "client-credentials");
    });
});

services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.UseSecurityHeaders();

//app.MapOpenApi(); // /openapi/v1.json
app.MapOpenApi("/openapi/v1/openapi.json");
//app.MapOpenApi("/openapi/{documentName}/openapi.json");

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
});

app.MapControllers();

app.Run();

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }
        document.Info = new()
        {
            Title = "My API Bearer scheme",
            Version = "v1",
            Description = "API for Damien"
        };
    }
}