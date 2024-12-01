using Microsoft.AspNetCore.Identity;
using UserAuthenticationTemplate.Models;
using UserAuthenticationTemplate.Services;

namespace UserAuthenticationTemplate.Tests.Mocks
{
    internal class MockUserManager : IUserManager<ApplicationUser>
    {
        private List<ApplicationUser> _users = [];

        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return Task.FromResult(user.PasswordHash == password);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            var userExists = _users.Exists(u => u.Email == user.Email);
            if (userExists)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User already exists!" }));

            user.PasswordHash = password;
            _users.Add(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user);
        }

        public Task<ApplicationUser?> FindByIdAsync(string id)
        {
            var user = _users.FirstOrDefault(u => u.Id.ToString() == id);
            return Task.FromResult(user);
        }

        public Task<ApplicationUser?> FindByNameAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.UserName == username);
            return Task.FromResult(user);
        }
    }
}
