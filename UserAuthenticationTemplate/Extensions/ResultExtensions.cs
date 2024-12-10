using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> CreateResult<T>(this T? data, string failureMessage = "Data must not be null")
        {
            return data != null
                ? Result<T>.Success(data)
                : Result<T>.Failure(failureMessage);
        }
    }
}