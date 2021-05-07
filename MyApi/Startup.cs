using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System;

namespace MyApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // IdentityModelEventSource.ShowPII = true;

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
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "AzureAd", "myADscheme");

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
                    {securityScheme, new string[] { }}
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
                    },
                });
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
