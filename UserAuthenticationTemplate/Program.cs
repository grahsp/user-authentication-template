namespace UserAuthenticationTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // -- Service --
            var builder = WebApplication.CreateBuilder(args);

            
            // -- Middleware --
            var app = builder.Build();

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
