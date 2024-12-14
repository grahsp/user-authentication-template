using System.Security.Claims;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Services
{
    public interface ITokenService
    {
        TokenResult GenerateToken(IEnumerable<Claim> claims);
        TokenValidationResult ValidateToken(string token);
    }
}
