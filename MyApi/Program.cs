using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using MyApi;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

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

services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    // add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "My API",
        Contact = new OpenApiContact
        {
            Name = "damienbod",
            Email = string.Empty,
            Url = new Uri("https://damienbod.com/"),
        }
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
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

app.UseSecurityHeaders(
    SecurityHeadersDefinitions.GetHeaderPolicyCollection(app.Environment.IsDevelopment()));

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API");
    c.RoutePrefix = string.Empty;
});

// only needed for browser clients
// app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
