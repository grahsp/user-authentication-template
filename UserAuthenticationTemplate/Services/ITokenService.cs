using System.Security.Claims;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Services
{
    /// <summary>
    /// Provides functionality for generating and validating tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a token based on the provided claims.
        /// </summary>
        /// <param name="claims">A collection of claims that the token will include.</param>
        /// <returns>A <see cref="TokenResult"/> containing the generated token and associated metadata.</returns>
        TokenResult GenerateToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Validates the provided token and returns the validation result.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>A <see cref="TokenValidationResult"/> indicating whether the token is valid and any associated claims.</returns>
        TokenValidationResult ValidateToken(string token);
    }
}
