using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IdentityDemo
{
    public class ShouldBeApp : IAuthorizationRequirement
    
    {
        
    }
    public class ShouldBeAppHandler : AuthorizationHandler<ShouldBeApp>
    {
        protected override Task HandleRequirementAsync(
           AuthorizationHandlerContext context,
           ShouldBeApp requirement)
        {
            //// check if Role claim exists -Else Return
            //// (sort of Claim-based requirement)
            //if (!context.User.HasClaim(x => x.Type == ClaimTypes.Role))
            //    return Task.CompletedTask;

            //// claim exists - retrieve the value
            //var claim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            //var role = claim.Value;

            //// check if the claim equals to App
            //// if satisfied, set the requirement as success
            //if (role == "App")
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
