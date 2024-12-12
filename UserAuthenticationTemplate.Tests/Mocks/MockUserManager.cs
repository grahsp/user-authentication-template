using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
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
            if (user == null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Missing user data" }));

            var userExists = _users.Exists(u => u.Email == user.Email || u.UserName == u.UserName);
            if (userExists)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User already exists!" }));

            if (password.Length < _identityConfig.Password.RequiredLength)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = $"Password must be atleast {_identityConfig.Password.RequiredLength} characters long!" }));

            if (_identityConfig.Password.RequireLowercase && !Regex.IsMatch(password, "[a-z]"))
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Password requires a lowercase character!" }));

            if (_identityConfig.Password.RequireUppercase && !Regex.IsMatch(password, "[A-Z]"))
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Password requires a uppercase character!" }));

            if (_identityConfig.Password.RequireDigit && !Regex.IsMatch(password, "[0-9]"))
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Password requires a digit!" }));

            if (_identityConfig.Password.RequireNonAlphanumeric && !Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Password requires a non-alphanumeric character!" }));

            var uniqueChars = new HashSet<char>(password);
            if (uniqueChars.Count < _identityConfig.Password.RequiredUniqueChars)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = $"Password requires atleast {_identityConfig.Password.RequiredUniqueChars} unique characters!" }));

            if (user.Email?.Length > 255)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Email too long" }));

            if (user.UserName?.Length > 255)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Username too long" }));

            if (password.Length > 255)
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Password too long" }));

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
