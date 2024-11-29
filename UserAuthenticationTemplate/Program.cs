using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserAuthenticationTemplate.Configs;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Data;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // -- Service --
            var builder = WebApplication.CreateBuilder(args);

            // Configurations
            var identityConfigurationSection = builder.Configuration.GetSection("Identity");
            builder.Services.Configure<IdentityConfig>(identityConfigurationSection);
            var jwtConfigurationSection = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtConfig>(jwtConfigurationSection);

            // This should be removed before moving to production and content inside of
            // appsettings.Secret.json Should be stored in a more secure way.
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Secret.json", optional: false, reloadOnChange: true);



            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbConnectionString"));
            });

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


            // -- Middleware --
            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.Run();
        }
    }
}
