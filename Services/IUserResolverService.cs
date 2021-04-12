using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace leashed.Services
{
    public interface ITokenUserResolverService
    {
        bool isAdmin();
        string getSub();
    }
}