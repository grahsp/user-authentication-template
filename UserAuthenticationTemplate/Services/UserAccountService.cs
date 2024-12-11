using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Extensions;
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
            var findUserResult = await FindUserAsync(request);
            if (findUserResult.IsFailure)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid login attempt." });

            var user = findUserResult.Data;

            if (await IsUserLockedOutAsync(user))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Your account is temporarily locked due to multiple failed login attempts. Please try again later." });
            }

            var checkPasswordResult = await CheckPasswordAsync(user, request.Password);
            if (checkPasswordResult.IsSuccess)
            {
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

        private async Task<Result> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var userIdentifierResult = GetUserIdentifier(user);
            if (userIdentifierResult.IsFailure)
                return userIdentifierResult.ToBase();

            var userIdentifier = userIdentifierResult.Data;
            try
            {
                var isSuccess = await _userManager.CheckPasswordAsync(user, password);
                _logger.LogChecKPasswordResult(isSuccess, userIdentifier, "Invalid password");

                return isSuccess.ToResult("Invalid password");
            }
            catch(Exception ex)
            {
                _logger.LogChecKPasswordResult(false, userIdentifier, ex.Message);
                return Result.Failure($"An error occurred trying to login.");
            }
        }

        private async Task<Result> IsUserLockedOutAsync(ApplicationUser user)
        {
            var userIdentifierResult = GetUserIdentifier(user);
            if (userIdentifierResult.IsFailure)
                return userIdentifierResult.ToBase();

            var userIdentifier = userIdentifierResult.Data;
            try
            {
                // TODO: Enhance response and logging to differentiate between locked-out, suspended, or banned states 
                // if functionality for suspensions or bans is implemented in the future.

                if (LockoutEnabled)
                {
                    _logger.LogInformation("User Lockout is disabled.");
                    return Result.Success();
                }

                // NOTE: The result is inverted because 'false' means the user is NOT locked out but result should be a success - confusing I know.
                var IsUserLockedOut = await _userManager.IsLockedOutAsync(user);
                IsUserLockedOut = !IsUserLockedOut;

                _logger.LogIsLockedOutResult(IsUserLockedOut, userIdentifier);
                return IsUserLockedOut.ToResult("User has been temporarily locked out. Try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogIsLockedOutResult(false, userIdentifier, ex.Message);
                return Result.Failure("An error occurred trying to login. Try again later.");
            }
        }

        private async Task<Result> AccessFailedCountAsync(ApplicationUser user)
        {
            var userIdentifierResult = GetUserIdentifier(user);
            if (userIdentifierResult.IsFailure)
                return userIdentifierResult.ToBase();

            var userIdentifier = userIdentifierResult.Data;
            try
            {
                var result = await _userManager.AccessFailedCountAsync(user);
                _logger.LogAccessFailedCountResult(result.Succeeded, userIdentifier);

                return result.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogAccessFailedCountResult(false, userIdentifier, ex.Message);
                return Result.Failure("An error occurred trying to login. Try again later.");
            }
        }

        private Result<string> GetUserIdentifier(ApplicationUser user)
        {
            if (user == null)
            {
                _logger.LogArgumentNull(nameof(user));
                return Result<string>.Failure("User is null. Please provide a valid user.");
            }

            if (string.IsNullOrEmpty(user.Email) && string.IsNullOrEmpty(user.UserName))
            {
                _logger.LogArgumentNull(nameof(user.Email), nameof(user.UserName));
                return Result<string>.Failure("Missing a user identifier. Please provide a valid email or username.");
            }

            // Implicitly converts string into Result<string>
            return user.UserName ?? user.Email ?? user.Id.ToString();
        }
    }
}
