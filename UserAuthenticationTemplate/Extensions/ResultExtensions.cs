using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Extensions
{
    public static class ResultExtensions
    {
        /// <summary>
        /// Converts a boolean into a Result with either a success or failure.
        /// </summary>
        /// <param name="isSuccess">The result of the operation (true for success, false for failure).</param>
        /// <param name="errorMessage">Error message to include in the result (only used if success is false).</param>
        /// <returns>A Result indicating success or failure with an optional failure message.</returns>
        public static Result ToResult(this bool isSuccess, string failureMessage)
        {
            return isSuccess ? Result.Success() : Result.Failure(failureMessage);
        }

        public static Result<T> ToResult<T>(this T? data, string failureMessage)
        {
            return data != null ? Result<T>.Success(data) : Result<T>.Failure(failureMessage);
        }
    }
}