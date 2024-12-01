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
            var userManager = new MockUserManager();
            _logger = new MockLogger<UserAccountService>();
            _userAccount = new(userManager, _logger, Options.Create(_identityConfig));
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldSucceed_IfValidRequest()
        {
            var registrationRequest = new RegistrationRequest
            {
                Email = "test@gmail.com",
                Password = "test123",
                ConfirmPassword = "test123"
            };

            var result = await _userAccount.RegisterUserAsync(registrationRequest);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldFail_IfInvalidRequest()
        {
            var registrationRequest = new RegistrationRequest
            {
                Email = "test@gmail.com",
                Password = "test123",
                ConfirmPassword = "test12"
            };

            var result = await _userAccount.RegisterUserAsync(registrationRequest);

            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldFail_IfUserExists()
        {
            var registrationRequest = new RegistrationRequest
            {
                Email = "test@gmail.com",
                Password = "test123",
                ConfirmPassword = "test123"
            };

            var result1 = await _userAccount.RegisterUserAsync(registrationRequest);
            Assert.IsTrue(result1.Succeeded);

            var result2 = await _userAccount.RegisterUserAsync(registrationRequest);
            Assert.IsFalse(result2.Succeeded);
            Assert.IsTrue(result2.Errors.Any(e => e.Description == "User already exists!"));
        }
    }
}
