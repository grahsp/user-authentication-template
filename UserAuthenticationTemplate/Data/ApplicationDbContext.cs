using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.NormalizedUserName)
                .IsUnique();

            builder.Entity<ApplicationUser>()
                .Property(u => u.Email)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique();


            // Role
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
                new ApplicationRole { Id = Guid.NewGuid(), Name = "Moderator", NormalizedName = "MODERATOR" },
                new ApplicationRole { Id = Guid.NewGuid(), Name = "User", NormalizedName = "USER" },
                new ApplicationRole { Id = Guid.NewGuid(), Name = "Guest", NormalizedName = "GUEST" }
            );

            builder.Entity<ApplicationRole>()
                .HasIndex(r => r.NormalizedName)
                .IsUnique();
        }
    }
}
