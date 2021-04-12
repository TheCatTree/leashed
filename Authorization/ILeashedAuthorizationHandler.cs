using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace leashed.Authorization
{
    public class ILeashedAuthorizationHandler : AuthorizationHandler<ILeashedAuthorizationHandlerRequirement>
    {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ILeashedAuthorizationHandlerRequirement requirement)
    {
        var permission = context.User?.Claims?.FirstOrDefault(x => x.Type == "permissions" && x.Value == requirement.ValidPermission);
        if (permission != null)
            context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
    }

    public class ILeashedAuthorizationHandlerRequirement : IAuthorizationRequirement
    {
        public string ValidPermission = Permissions.Admin;
    }

    public class Permissions{
        public const string Admin = "role:admin";
        
    }
    
}