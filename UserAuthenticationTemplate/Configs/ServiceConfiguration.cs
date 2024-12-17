using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Services;

namespace UserAuthenticationTemplate.Configs
{
    public static class ServiceConfiguration
    {
        // Identity
        public static void FromConfig(this IdentityOptions options, IConfiguration configuration, Action<IdentityConfig>? customConfiguration = null)
        {
            var config = configuration.GetSection("Identity").Get<IdentityConfig>() ?? new IdentityConfig();
            customConfiguration?.Invoke(config);

            // Password
            var password = config.Password;

            options.Password.RequireDigit = password.RequireDigit;
            options.Password.RequireLowercase = password.RequireLowercase;
            options.Password.RequireUppercase = password.RequireUppercase;
            options.Password.RequireNonAlphanumeric = password.RequireNonAlphanumeric;
            options.Password.RequiredLength = password.RequiredLength;
            options.Password.RequiredUniqueChars = password.RequiredUniqueChars;

            // Lockout
            var lockout = config.Lockout;

            options.Lockout.AllowedForNewUsers = lockout.AllowedForNewUsers;
            options.Lockout.MaxFailedAccessAttempts = lockout.MaxFailedAccessAttempts;
            options.Lockout.DefaultLockoutTimeSpan = lockout.DefaultLockoutTimeSpan;

            // Sign in
            var signIn = config.SignIn;

            options.SignIn.RequireConfirmedEmail = signIn.RequireConfirmedEmail;
            options.SignIn.RequireConfirmedPhoneNumber = signIn.RequireConfirmedPhoneNumber;
            options.SignIn.RequireConfirmedAccount = signIn.RequireConfirmedPhoneNumber;

            // User
            var user = config.User;

            options.User.AllowedUserNameCharacters = user.AllowedUsernameCharacters;
        }

        // JWT
        public static void FromConfig(this JwtBearerOptions options, IConfiguration configuration, Action<JwtConfig>? customConfiguration = null)
        {
            var jwtConfig = configuration.GetSection("Jwt").Get<JwtConfig>() ?? new JwtConfig();
            customConfiguration?.Invoke(jwtConfig);

            if (string.IsNullOrEmpty(jwtConfig?.Secret))
                throw new InvalidOperationException("JWT Secret is missing from configuration!");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtConfig.ValidateIssuer,
                ValidateAudience = jwtConfig.ValidateAudience,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                ClockSkew = jwtConfig.ClockSkew,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
            };
        }

        public static void AddRequiredServices<TUser>(this IServiceCollection services) where TUser : class
        {
            services.AddScoped<ITokenService, JwtService>();
            services.AddScoped<IUserManager<TUser>, UserAccountManager<TUser>>();
            services.AddScoped<UserAccountService>();
        }
    }
}
