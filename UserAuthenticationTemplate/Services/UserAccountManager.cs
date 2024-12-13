using Microsoft.AspNetCore.Identity;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Services
{
    public class UserAccountManager : IUserManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAccountManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityResult> AccessFailedCountAsync(ApplicationUser user)
        {
            return _userManager.AccessFailedAsync(user);
        }

        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<ApplicationUser?> FindByIdAsync(string id)
        {
            return _userManager.FindByIdAsync(id);
        }

        public Task<ApplicationUser?> FindByNameAsync(string username)
        {
            return _userManager.FindByNameAsync(username);
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return _userManager.GetAccessFailedCountAsync(user);
        }

        public Task<bool> IsLockedOutAsync(ApplicationUser user)
        {
            return _userManager.IsLockedOutAsync(user);
        }

        public Task<IdentityResult> ResetAccessFailedCountAsync(ApplicationUser user)
        {
            return _userManager.ResetAccessFailedCountAsync(user);
        }
    }
}
