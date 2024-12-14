using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserAuthenticationTemplate.Configs;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Data;
using UserAuthenticationTemplate.Models;
using UserAuthenticationTemplate.Services;

namespace UserAuthenticationTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // -- Service Configuration -- //
            var builder = WebApplication.CreateBuilder(args);

            #region Configuration
            builder.Configuration.AddUserSecrets<Program>();

            var identityConfigurationSection = builder.Configuration.GetSection("Identity");
            builder.Services.Configure<IdentityConfig>(identityConfigurationSection);
            var jwtConfigurationSection = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtConfig>(jwtConfigurationSection);
            #endregion

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                // IMPORTANT: Change connection string to whatever name you've saved it as!
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbConnectionString"));
            });

            #region Register Services
            builder.Services.AddScoped<ITokenService, JwtService>();
            builder.Services.AddScoped<IUserManager<ApplicationUser>, UserAccountManager>();
            builder.Services.AddScoped<UserAccountService>();

            builder.Services.AddControllers();
            #endregion

            #region Identity Setup
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                var identityConfig = identityConfigurationSection.Get<IdentityConfig>() ?? new IdentityConfig();

                // Password
                var password = identityConfig.Password;

                options.Password.RequireDigit = password.RequireDigit;
                options.Password.RequireLowercase = password.RequireLowercase;
                options.Password.RequireUppercase = password.RequireUppercase;
                options.Password.RequireNonAlphanumeric = password.RequireNonAlphanumeric;
                options.Password.RequiredLength = password.RequiredLength;
                options.Password.RequiredUniqueChars = password.RequiredUniqueChars;

                // Lockout
                var lockout = identityConfig.Lockout;

                options.Lockout.AllowedForNewUsers = lockout.AllowedForNewUsers;
                options.Lockout.MaxFailedAccessAttempts = lockout.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = lockout.DefaultLockoutTimeSpan;

                // Sign in
                var signIn = identityConfig.SignIn;

                options.SignIn.RequireConfirmedEmail = signIn.RequireConfirmedEmail;
                options.SignIn.RequireConfirmedPhoneNumber = signIn.RequireConfirmedPhoneNumber;
                options.SignIn.RequireConfirmedAccount = signIn.RequireConfirmedPhoneNumber;

                // User
                var user = identityConfig.User;

                options.User.AllowedUserNameCharacters = user.AllowedUsernameCharacters;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            #endregion

            #region Authentication Setup
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtConfig = jwtConfigurationSection.Get<JwtConfig>();

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
            });

            builder.Services.AddAuthorization();
            #endregion


            // -- Middleware Configuration -- //
            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
