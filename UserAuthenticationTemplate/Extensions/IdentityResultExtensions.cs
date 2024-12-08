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

        public static Result<T> ToResult<T>(this IdentityResult result, T Data)
        {
            return new Result<T>(result.Succeeded, Data, result.Errors.Select(e => e.Description).ToArray());
        }
    }
}
