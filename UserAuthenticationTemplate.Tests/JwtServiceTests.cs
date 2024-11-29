using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthenticationTemplate.Configs;
using UserAuthenticationTemplate.Services;
namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class JwtServiceTests
    {
        #region GenerateToken
        [TestMethod]
        public void GenerateToken_ShouldReturnValidToken()
        {
            // Arrange
            var config = new JwtConfig { Secret = GenerateSecret(), Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 15 };
            var jwtService = new JwtService(Options.Create(config));
            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, "Testington")
            };
            // Act
            var token = jwtService.GenerateToken(claims);
            // Assert
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Split('.').Length == 3);
        }
        [TestMethod]
        public void GenerateToken_ShouldIncludeExpiration_WhenExpiresInSet()
        {
            // Arrange
            var config = new JwtConfig { Secret = GenerateSecret(), Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 15 };
            var jwtService = new JwtService(Options.Create(config));
            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, "Testington")
            };
            // Act
            var token = jwtService.GenerateToken(claims);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            // Assert
            Assert.IsTrue(jwtToken.ValidTo > DateTime.UtcNow);
        }
        #endregion
        #region ValidateToken
        [TestMethod]
        public void ValidateToken_ShouldReturnTrue_ForValidToken()
        {
            // Arrange
            var config = new JwtConfig { Secret = GenerateSecret(), Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 15 };
            var jwtService = new JwtService(Options.Create(config));
            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, "Testington")
            };
            var token = jwtService.GenerateToken(claims);
            // Act
            var isValid = jwtService.ValidateToken(token);
            // Assert
            Assert.IsTrue(isValid);
        }
        [TestMethod]
        public void ValidateToken_ShouldReturnFalse_ForExpiredToken()
        {
            // Arrange
            var config = new JwtConfig { Secret = GenerateSecret(), Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 0, ClockSkewInMinutes = 0 };
            var jwtService = new JwtService(Options.Create(config));
            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, "Testington")
            };
            var token = jwtService.GenerateToken(claims);
            // Act
            var isValid = jwtService.ValidateToken(token);
            // Assert
            Assert.IsFalse(isValid);
        }
        [TestMethod]
        public void ValidateToken_ShouldReturnFalse_ForInvalidSignature()
        {
            // Arrange
            var config = new JwtConfig { Secret = GenerateSecret(), Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 0, ClockSkewInMinutes = 0 };
            var jwtService = new JwtService(Options.Create(config));
            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, "Testington")
            };
            var invalidToken = jwtService.GenerateToken(claims) + "a";
            // Act
            var isValid = jwtService.ValidateToken(invalidToken);
            // Assert
            Assert.IsFalse(isValid);
        }
        [TestMethod]
        public void ValidateToken_ShouldReturnFalse_ForInvalidIssuer()
        {
            // Arrange
            var config = new JwtConfig { Secret = GenerateSecret(), Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 0, ClockSkewInMinutes = 0 };
            var jwtService = new JwtService(Options.Create(config));
            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, "Testington")
            };
            var token = jwtService.GenerateToken(claims);
            // Act: Modify the token to have an invalid issuer claim
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Payload["iss"] = "invalid";
            // Recreate the token with the modified issuer claim
            var invalidToken = handler.WriteToken(jwtToken);
            var isValid = jwtService.ValidateToken(invalidToken);
            // Assert
            Assert.IsFalse(isValid);
        }
        [TestMethod]
        public void ValidateToken_ShouldReturnFalse_ForInvalidAudience()
        {
            // Arrange
            var config = new JwtConfig { Secret = GenerateSecret(), Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 0, ClockSkewInMinutes = 0 };
            var jwtService = new JwtService(Options.Create(config));
            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, "Testington")
            };
            var token = jwtService.GenerateToken(claims);
            // Act: Modify the token to have an invalid issuer claim
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Payload["aud"] = "invalid";
            // Recreate the token with the modified issuer claim
            var invalidToken = handler.WriteToken(jwtToken);
            var isValid = jwtService.ValidateToken(invalidToken);
            // Assert
            Assert.IsFalse(isValid);
        }
        #endregion
        private string GenerateSecret()
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890+-/_";
            var secret = new StringBuilder();
            for (var i = 0; i < 128; i++)
            {
                secret.Append(chars[new Random().Next(chars.Length)]);
            }
            return secret.ToString();
        }
    }
}