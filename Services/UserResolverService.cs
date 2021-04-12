using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System;
using leashed.Authorization;

namespace leashed.Services
{
    public class TokenUserResolverService : ITokenUserResolverService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Task<JwtSecurityToken> _jwtSecurityToken;
        public TokenUserResolverService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtSecurityToken = getSecurityTokenTask();
        }

        private async Task<JwtSecurityToken> getSecurityTokenTask()
        {
            var tokenSting  = await getTokenString();
            return getSecurityToken(tokenSting);
        }

        private async Task<string> getTokenString(){
            return  await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        }

        private JwtSecurityToken getSecurityToken(string token){
            var sTH = new JwtSecurityTokenHandler();
            return sTH.ReadJwtToken(token);
        }

        private IEnumerable<Claim> getClaims(){
            return _jwtSecurityToken.Result.Claims;
        }

        public bool isAdmin(){
            
             var premisions = getClaims().FirstOrDefault(x => x.Type == "permissions" && x.Value == Permissions.Admin);
            if(premisions != null)
                {return true;}
            
            return false;
        }

        public string getSub(){
            return getClaims().First(c => c.Type == "sub").Value;

        }

    }
}