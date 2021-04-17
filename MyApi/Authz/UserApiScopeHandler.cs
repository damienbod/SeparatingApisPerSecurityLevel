using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi
{
    public class UserApiScopeHandler : AuthorizationHandler<UserApiScopeHandlerRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserApiScopeHandlerRequirement requirement)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (requirement == null)
                throw new ArgumentNullException(nameof(requirement));

            var scopeClaim = context.User.Claims.FirstOrDefault(t => t.Type == "scope");

            if (scopeClaim != null)
            {
                var scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (scopes.Any(t => t == "auth0-user-api-one"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    public class UserApiScopeHandlerRequirement : IAuthorizationRequirement
    { }
}