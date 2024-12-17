using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserAuthenticationTemplate.Configs;
using UserAuthenticationTemplate.Data;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // -- Service Configuration -- //
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddUserSecrets<Program>();

            builder.Services.AddControllers();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                // IMPORTANT: Change connection string to whatever name you've saved it as!
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbConnectionString"));
            });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                options.FromConfig(builder.Configuration)
            )
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.FromConfig(builder.Configuration);
            });

            builder.Services.AddAuthorization();


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
