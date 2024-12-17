using Microsoft.AspNetCore.Identity;

namespace UserAuthenticationTemplate.Services
{
    public class UserAccountManager<TUser>(UserManager<TUser> userManager) : IUserManager<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager = userManager;

        public Task<IdentityResult> AccessFailedCountAsync(TUser user)
        {
            return _userManager.AccessFailedAsync(user);
        }

        public Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<TUser?> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<TUser?> FindByIdAsync(string id)
        {
            return _userManager.FindByIdAsync(id);
        }

        public Task<TUser?> FindByNameAsync(string username)
        {
            return _userManager.FindByNameAsync(username);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return _userManager.GetAccessFailedCountAsync(user);
        }

        public Task<bool> IsLockedOutAsync(TUser user)
        {
            return _userManager.IsLockedOutAsync(user);
        }

        public Task<IdentityResult> ResetAccessFailedCountAsync(TUser user)
        {
            return _userManager.ResetAccessFailedCountAsync(user);
        }
    }
}
