using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace leashed.Authorization.ExtensionMethods
{
    public static class AuthorizationExtensions
    {
        public static bool IsAdmin(this IEnumerable<Claim> claims){
            var premisions = claims.FirstOrDefault(x => x.Type == "permissions" && x.Value == Permissions.Admin);
            if(premisions != null)
                {return true;}
            return false;
        }
    }
}