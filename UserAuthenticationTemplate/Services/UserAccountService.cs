using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Services
{
    public class UserAccountService
    {
        private readonly IUserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserAccountService> _logger;
        private readonly IdentityConfig _identityConfig;

        private bool LockoutEnabled => _identityConfig.Lockout.Enabled;

        public UserAccountService(IUserManager<ApplicationUser> userManager, ILogger<UserAccountService> logger, IOptions<IdentityConfig> identityConfig)
        {
            _userManager = userManager;
            _logger = logger;
            _identityConfig = identityConfig.Value;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegistrationRequest request)
            {
            if (!string.Equals(request.Password, request.ConfirmPassword))
            {
                _logger.LogWarning("Passwords did not match during registration attempt.");
                return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
            }

            var newUser = new ApplicationUser
            {
                Email = request.Email
            };

            // Handle success vs failure here..
            var result = await CreateAsync(newUser, request.Password);

            return result;
        }

        private async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID '{UserId}' successfully created.", user.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to create user with ID '{UserId}'", user.Id);

                    foreach (var error in result.Errors)
                    {
                        _logger.LogWarning(" - {ErrorDescription}", error.Description);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to create user with ID '{UserId}'.", user.Id);
                return IdentityResult.Failed(new IdentityError { Description = "An error occurred while creating your account. Please try again later." });
            }
        }

        public async Task<IdentityResult> LoginUserAsync(LoginRequest request)
        {
            var user = await FindUserAsync(request.Email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid login attempt." });
            }

            if (await IsUserLockedOutAsync(user))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Your account is temporarily locked due to multiple failed login attempts. Please try again later." });
            }

            if (await CheckPasswordAsync(user, request.Password))
            {
                // Success
                return IdentityResult.Success;
            }
            else
            {
                _ = await AccessFailedCountAsync(user);
                return IdentityResult.Failed(new IdentityError { Description = "Invalid login attempt. Please check your username and password and try again." });
            }
        }

        private async Task<ApplicationUser?> FindUserAsync(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                _logger.LogError("Parameter identifier cannot be empty or null.");
                return null;
            }

            try
            {
                ApplicationUser? user;
                if (IsValidEmail(identifier))
                {
                    user = await _userManager.FindByEmailAsync(identifier);
                    if (user == null)
                        _logger.LogWarning("No user found with Email: '{Identifier}'", identifier);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(identifier);
                    if (user == null)
                        _logger.LogWarning("No user found with Username: '{Identifier}'", identifier);
                }

                if (user != null)
                    _logger.LogInformation("User found with ID '{UserId}'", user.Id);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to find user with identifier '{Identifier}'.", identifier);
                return null;
            }
        }

        private static bool IsValidEmail(string identifier)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(identifier, emailPattern);
        }

        private async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userManager.CheckPasswordAsync(user, password);
                if (result)
                {
                    _logger.LogInformation("Successfully authenticated user with ID '{UserId}'.", user.Id);
                }
                else
                {
                    _logger.LogWarning("Incorrect password provided for user with ID '{UserId}'.", user.Id);
                }

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while comparing passwords for user with ID '{UserId}'.", user.Id);
                return false;
            }
        }

        private async Task<bool> IsUserLockedOutAsync(ApplicationUser user)
        {
            try
            {
                var isLockedOut = await _userManager.IsLockedOutAsync(user) && LockoutEnabled;
                if (isLockedOut)
                {
                    _logger.LogInformation("User with ID '{UserId}' is locked out.", user.Id);
                }
                else
                {
                    _logger.LogInformation("User with ID '{UserId}' is not locked out.", user.Id);
                }

                return isLockedOut;
            }
            catch (Exception ex)
            {
                // This will allow blocked users in if an error occurs (change result type or re-throw)
                _logger.LogError(ex, "An error occurred while checking lockout state for user with ID '{UserId}'.", user.Id);
                return false;
            }
        }

        private async Task<bool> AccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.AccessFailedCountAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully retrieved failed login count for user with ID '{UserId}'.", user.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve failed login count for user with ID '{UserId}'.", user.Id);
                }

                return result.Succeeded && LockoutEnabled;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while accessing the failed login count for user with ID '{UserId}'.", user.Id);
                return false;
            }
        }
    }
}
