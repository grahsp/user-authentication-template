using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthenticationTemplate.Configs;
namespace UserAuthenticationTemplate.Services
{
    public class JwtService
    {
        private JwtConfig _config;

        public JwtService(IOptions<JwtConfig> config)
        {
            _config = config.Value;
        }

        public string GenerateToken(IEnumerable<Claim> claims, DateTime? expires = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret!));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: expires ?? DateTime.UtcNow.AddMinutes(_config.ExpiresInMinutes),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public bool ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret!));

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

                new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var validatedToken);

                return validatedToken != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}