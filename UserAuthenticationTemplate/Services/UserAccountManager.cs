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
    }
}
