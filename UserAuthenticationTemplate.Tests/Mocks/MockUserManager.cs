using Microsoft.AspNetCore.Identity;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Models;
using UserAuthenticationTemplate.Services;

namespace UserAuthenticationTemplate.Tests.Mocks
{
    internal class MockUserManager : IUserManager<ApplicationUser>
    {
        private List<ApplicationUser> _users = [];
        private readonly IdentityConfig _identityConfig;

        public MockUserManager(IdentityConfig identityConfig)
        {
            _identityConfig = identityConfig;
        }

        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return Task.FromResult(user.PasswordHash == password);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            var userExists = _users.Exists(u => u.Email == user.Email);
            if (userExists)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User already exists!" }));

            user.Id = Guid.NewGuid();
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

        public async Task<IdentityResult> AccessFailedCountAsync(ApplicationUser user)
        {
            var selectUser = await FindByEmailAsync(user.Email!);

            if (selectUser != null)
            {
                selectUser.AccessFailedCount++;
                if (selectUser.AccessFailedCount >= _identityConfig.Lockout.MaxFailedAccessAttempts)
                {
                    selectUser.LockoutEnd = DateTimeOffset.Now.AddMinutes(_identityConfig.Lockout.DefaultLockoutTimeSpan.TotalMinutes);
                }

                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<bool> IsLockedOutAsync(ApplicationUser user)
        {
            var selectUser = await FindByEmailAsync(user.Email!);

            return selectUser?.LockoutEnd > DateTime.Now;
        }
    }
}
