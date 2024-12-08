using Microsoft.AspNetCore.Identity;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Result ToResult(this IdentityResult result)
        {
            return new Result(result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }
    }
}
