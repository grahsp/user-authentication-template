﻿using Microsoft.Extensions.Options;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Extensions;
using UserAuthenticationTemplate.Logging;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Services
{
    public class UserAccountService(IUserManager<ApplicationUser> userManager, ILogger<UserAccountService> logger, IOptions<IdentityConfig> identityConfig)
    {
        private readonly IUserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<UserAccountService> _logger = logger;
        private readonly IdentityConfig _identityConfig = identityConfig.Value;

        private bool LockoutEnabled => _identityConfig.Lockout.Enabled;


        public async Task<Result<RegisterResponse>> RegisterUserAsync(RegistrationRequest request)
        {
            var userToRegister = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName ?? request.Email,
            };

            var createResult = await CreateAsync(userToRegister, request.Password);
            if (createResult.IsFailure)
                return Result<RegisterResponse>.Failure(createResult.Errors);

            return Result<RegisterResponse>.Success(new RegisterResponse());  // Placeholder
        }

        private async Task<Result> CreateAsync(ApplicationUser user, string password)
        {
            var userIdentifierResult = GetUserIdentifier(user);
            if (userIdentifierResult.IsFailure)
                return userIdentifierResult.ToBase();

            var userIdentifier = userIdentifierResult.Data;
            try
            {
                var identityResult = await _userManager.CreateAsync(user, password);
                _logger.LogCreateUserResult(identityResult.Succeeded, userIdentifier);

                return identityResult.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogCreateUserResult(false, userIdentifier, ex.Message);
                return Result.Failure("An error occurred trying to register user.");
            }
        }

        public async Task<Result<LoginResponse>> LoginUserAsync(LoginRequest request)
        {
            var findUserResult = await FindUserAsync(request);
            if (findUserResult.IsFailure)
                return Result<LoginResponse>.Failure(findUserResult.Errors);

            var user = findUserResult.Data;

            var lockoutResult = await IsUserLockedOutAsync(user);
            if (lockoutResult.IsFailure)
                return Result<LoginResponse>.Failure(lockoutResult.Errors);

            var passwordResult = await CheckPasswordAsync(user, request.Password);
            if (passwordResult.IsSuccess)
            {
                // -- Success --
                return Result<LoginResponse>.Success(new LoginResponse());  // Placeholder
            }

            var failedCountResult = await AccessFailedCountAsync(user);
            if (failedCountResult.IsFailure)
                return Result<LoginResponse>.Failure(failedCountResult.Errors);

            _logger.LogError("An unexpected error occurred trying to log user in.");
            return Result<LoginResponse>.Failure("An unexpected error occurred!");
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
