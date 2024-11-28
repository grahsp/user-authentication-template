using UserAuthenticationTemplate.Configs.Identity;

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

            // This should be removed before moving to production and content inside of
            // appsettings.Secret.json Should be stored in a more secure way.
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Secret.json", optional: false, reloadOnChange: true);



            // -- Middleware --
            var app = builder.Build();

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
