using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Services
{
    public class UserAccountService
    {
        private readonly IUserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserAccountService> _logger;
        private readonly IdentityConfig _identityConfig;

        public UserAccountService(IUserManager<ApplicationUser> userManager, ILogger<UserAccountService> logger, IOptions<IdentityConfig> identityConfig)
        {
            _userManager = userManager;
            _logger = logger;
            _identityConfig = identityConfig.Value;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegistrationRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });

            var newUser = new ApplicationUser
            {
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (result.Succeeded)
            {
                // Logic that should execute if successful goes here..
                _logger.LogInformation("User '{Email}' successfully registered.", newUser.Email);
            }
            else
            {
                _logger.LogWarning("User '{Email}' failed to register!", newUser.Email);
            }

            return result;
        }
    }
}
