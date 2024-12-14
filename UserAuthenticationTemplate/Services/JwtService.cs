using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthenticationTemplate.Configs;
using UserAuthenticationTemplate.Models;
using UserAuthenticationTemplate.Shared.Enums;
namespace UserAuthenticationTemplate.Services
{
    /// <summary>
    /// Provides functionality for creating, validating, and managing JSON Web Tokens.
    /// </summary>
    /// <remarks>
    /// This service is designed to handle JWT operations using the configuration provided via <see cref="JwtConfig"/>.
    /// It supports token generation, validation, and other security-related operations.
    /// </remarks>
    public class JwtService : ITokenService
    {
        private readonly JwtConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class using the specified configuration.
        /// </summary>
        /// <param name="config">The JWT configuration options.</param>
        public JwtService(IOptions<JwtConfig> config)
        {
            if (string.IsNullOrEmpty(config.Value.Secret))
                throw new ArgumentNullException(nameof(config), "Jwt secret cannot be empty or null!");

            _config = config.Value;
        }

        /// <summary>
        /// Generates a JWT token with the specified claims and expiration time.
        /// </summary>
        /// <param name="claims">
        /// A collection of claims to include in the token. Claims provide information about the user or entity being authenticated.
        /// </param>
        /// <returns>
        /// A string representation of the generated JWT.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the configuration's secret key is null or empty.
        /// </exception>
        /// <remarks>
        /// This method uses HMAC SHA-256 for signing the token and the issuer, audience, and expiration are configured
        /// using the <see cref="JwtConfig"/> object.
        /// </remarks>
        public TokenResult GenerateToken(IEnumerable<Claim> claims)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret!));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtToken = new JwtSecurityToken(
                    issuer: _config.Issuer,
                    audience: _config.Audience,
                    claims: claims,
                    expires: _config.Expires,
                    signingCredentials: signingCredentials
                );

                var data = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return TokenResult.Success(data);
            }
            catch (ArgumentNullException)
            {
                return TokenResult.Failure(SecurityError.InvalidConfiguration, "Invalid configuration for token generation.");
            }
            catch(Exception)
            {
                return TokenResult.Failure(SecurityError.UnexpectedError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Validates a JWT token against the configured parameters to ensure its integrity and authenticity.
        /// </summary>
        /// <param name="token">
        /// The JWT string to validate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the token is valid; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the configuration's secret key is null or empty.
        /// </exception>
        /// <remarks>
        /// The validation process checks the token's issuer, audience, signature, and lifetime based on the parameters defined in <see cref="JwtConfig"/>.
        /// If the token is invalid, the method catches the exception and returns <c>false</c> without exposing the details.
        /// </remarks>
        public Models.TokenValidationResult ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret!));

            static Models.TokenValidationResult InvalidToken()
            {
                return Models.TokenValidationResult.Failure(SecurityError.InvalidToken, "Your session is no longer valid. Please log in again to continue.");
            }

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidIssuer = _config.Issuer,
                    ValidAudience = _config.Audience,
                    ValidateIssuer = _config.ValidateIssuer,
                    ValidateAudience = _config.ValidateAudience,
                    ClockSkew = _config.ClockSkew,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key
                };

                new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var _);

                return Models.TokenValidationResult.Success();
            }
            catch(SecurityTokenExpiredException)
            {
                return Models.TokenValidationResult.Failure(SecurityError.ExpiredToken, "Your token has expired. Please refresh your token.");
            }
            catch(SecurityTokenInvalidAudienceException)
            {
                return InvalidToken();
            }
            catch(SecurityTokenInvalidIssuerException)
            {
                return InvalidToken();
            }
            catch(SecurityTokenInvalidLifetimeException)
            {
                return InvalidToken();
            }
            catch(SecurityTokenNoExpirationException)
            {
                return InvalidToken();
            }
            catch(SecurityTokenException)
            {
                return InvalidToken();
            }
            catch (Exception)
            {
                return Models.TokenValidationResult.Failure(SecurityError.UnexpectedError, "An unexpected error occured.");
            }
        }
    }
}