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
            {
                _logger.LogWarning("Failed to register '{Email}' due to a password missmatch", request.Email);
                return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
            }

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

        public async Task<IdentityResult> LoginUserAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("User '{Email}' not found.", request.Email);
                return IdentityResult.Failed(new IdentityError { Description = $"User '{ request.Email }' not found." });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (isPasswordValid)
            {
                // Logic that should execute if successful goes here..
                _logger.LogInformation("User '{Email}' succesfully logged in.", user.Email);

                return IdentityResult.Success;
            }
            else
            {
                _logger.LogWarning("User '{Email}' failed to login with incorrect password.", user.Email);
                return IdentityResult.Failed(new IdentityError { Description = $"User '{user.Email} failed to login." });
            }
        }
    }
}
