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
