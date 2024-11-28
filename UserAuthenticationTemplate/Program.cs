using Microsoft.EntityFrameworkCore;
using UserAuthenticationTemplate.Data;

namespace UserAuthenticationTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // -- Service --
            var builder = WebApplication.CreateBuilder(args);

            // This should be removed before moving to production and content inside of
            // appsettings.Secret.json Should be stored in a more secure way.
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Secret.json", optional: false, reloadOnChange: true);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbConnectionString"));
            });


            // -- Middleware --
            var app = builder.Build();

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
