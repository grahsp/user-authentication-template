using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Logging;
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
            var userIdentifier = request.Email ?? request.UserName ?? throw new ArgumentNullException(nameof(request), "Both email and username cannot be left empty!");
            
            var findUserResult = await FindUserAsync(request);
            if (findUserResult.IsFailure)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid login attempt." });

            var user = findUserResult.Data;

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

        private async Task<Result<ApplicationUser>> FindByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                _logger.LogFindByEmailResult(user != null, email);

                return Result<ApplicationUser>.FromData(user, $"Could not find user with email '{email}'.");
            }
            catch (Exception ex)
            {
                _logger.LogFindByEmailResult(false, email, ex.Message);
                return Result<ApplicationUser>.Failure(ex.Message);
            }
        }

        private async Task<Result<ApplicationUser>> FindByNameAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                _logger.LogFindByNameResult(user != null, username);

                return Result<ApplicationUser>.FromData(user, $"Could not find user with username '{username}'.");
            }
            catch (Exception ex)
            {
                _logger.LogFindByNameResult(false, username, ex.Message);
                return Result<ApplicationUser>.Failure(ex.Message);
            }
        }

        private async Task<Result<ApplicationUser>> FindUserAsync(LoginRequest request)
        {
            if (request.Email != null)
                return await FindByEmailAsync(request.Email);

            if (request.UserName != null)
                return await FindByNameAsync(request.UserName);

            _logger.LogArgumentNull(nameof(request.Email), nameof(request.UserName));
            return Result<ApplicationUser>.Failure("Request is missing email and username.");
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
