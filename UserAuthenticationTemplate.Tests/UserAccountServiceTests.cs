using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserAuthenticationTemplate.Configs.Identity;
using UserAuthenticationTemplate.Models;
using UserAuthenticationTemplate.Services;
using UserAuthenticationTemplate.Tests.Mocks;

namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class UserAccountServiceTests
    {
        private readonly UserAccountService _userAccount;
        private readonly ILogger<UserAccountService> _logger;
        private readonly IdentityConfig _identityConfig = new();

        public UserAccountServiceTests()
        {
            var userManager = new MockUserManager(_identityConfig);
            _logger = new MockLogger<UserAccountService>();
            _userAccount = new(userManager, _logger, Options.Create(_identityConfig));
        }

        #region Register
        [TestMethod]
        public async Task RegisterUserAsync_ShouldSucceed_IfValidRequest()
        {
            var result = await RegisterUserAsync("test@gmail.com", "test123");

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldFail_IfInvalidRequest()
        {
            var result = await RegisterUserAsync("test@gmail.com", "test123", "incorrect password");

            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldFail_IfUserExists()
        {
            var result1 = await RegisterUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(result1.Succeeded);

            var result2 = await RegisterUserAsync("test@gmail.com", "test123");
            Assert.IsFalse(result2.Succeeded);
            Assert.IsTrue(result2.Errors.Any(e => e.Description == "User already exists!"));
        }
        #endregion

        #region Login
        [TestMethod]
        public async Task LoginUserAsync_ShouldFail_IfUserUnknown()
        {
            var result = await LoginUserAsync("test@gmail.com", "test123");
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public async Task LoginUserAsync_ShouldFail_IfIncorrectPassword()
        {
            var registered = await RegisterUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(registered.Succeeded);

            var result = await LoginUserAsync("test@gmail.com", "test321");
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public async Task LoginUserAsync_ShouldSucceed_IfValidRequest()
        {
            var registered = await RegisterUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(registered.Succeeded);

            var result = await LoginUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(result.Succeeded);
        }

        // -- Lockout Tets --

        [TestMethod]
        public async Task LoginUserAsync_ShouldLock_IfTooManyFailedAttempts()
        {
            _identityConfig.Lockout.MaxFailedAccessAttempts = 2;
            _identityConfig.Lockout.DefaultLockoutInMinutes = 5;

            var registered = await RegisterUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(registered.Succeeded);

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts; i++)
            {
                await LoginUserAsync("test@gmail.com", "test321");
            }

            var result = await LoginUserAsync("test@gmail.com", "test123");

            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public async Task LoginUserAsync_ShoulSucceed_IfAttemptsAreLessThanMax()
        {
            _identityConfig.Lockout.MaxFailedAccessAttempts = 2;

            var registered = await RegisterUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(registered.Succeeded);

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts - 1; i++)
            {
                await LoginUserAsync("test@gmail.com", "test321");
            }

            var result = await LoginUserAsync("test@gmail.com", "test123");

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task LoginUserAsync_ShoulUnlockAfterLockoutEnds_IfAccountIsLocked()
        {
            var ms = 250;   // Milliseconds to lock user out and time to sleep
            _identityConfig.Lockout.MaxFailedAccessAttempts = 2;
            _identityConfig.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(ms);

            var registered = await RegisterUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(registered.Succeeded);

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts; i++)
            {
                await LoginUserAsync("test@gmail.com", "test321");
            }

            var resultBefore = await LoginUserAsync("test@gmail.com", "test123");
            Assert.IsFalse(resultBefore.Succeeded);

            Thread.Sleep(ms);

            var resultAfter = await LoginUserAsync("test@gmail.com", "test123");
            Assert.IsTrue(resultAfter.Succeeded);
        }
        #endregion

        #region Helper Methods
        private async Task<IdentityResult> RegisterUserAsync(string email, string password, string? confirmPassword = null)
        {
            var registrationRequest = new RegistrationRequest
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword ?? password
            };

            return await _userAccount.RegisterUserAsync(registrationRequest);
        }

        private async Task<IdentityResult> LoginUserAsync(string email, string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            return await _userAccount.LoginUserAsync(loginRequest);
        }
        #endregion
    }
}
