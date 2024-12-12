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
        private readonly IdentityConfig _identityConfig;

        public UserAccountServiceTests()
        {
            _identityConfig = new IdentityConfig
            {
                Password = new PasswordConfig
                {
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false,
                    RequiredLength = 0,
                    RequiredUniqueChars = 0,
                }
            };

            var userManager = new MockUserManager(_identityConfig);
            _logger = new MockLogger<UserAccountService>();
            _userAccount = new(userManager, _logger, Options.Create(_identityConfig));
        }

        #region Password Validation Tests
        // Password Required Length
        [TestMethod]
        public async Task RegisterUser_PasswordMeetsLengthRequirement_Success()
        {
            _identityConfig.Password.RequiredLength = 6;

            var result = await RegisterUserAsync(password: "longer");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public async Task RegisterUser_PasswordTooShort_FailureWithValidationError()
        {
            _identityConfig.Password.RequiredLength = 6;

            var result = await RegisterUserAsync(password: "short");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains($"password must be atleast {_identityConfig.Password.RequiredLength} characters long", StringComparison.OrdinalIgnoreCase)));
        }

        // Password Requires Lowercase
        [TestMethod]
        public async Task RegisterUser_PasswordContainsLowercase_Success()
        {
            _identityConfig.Password.RequireLowercase = true;

            var result = await RegisterUserAsync(password: "lowercase");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public async Task RegisterUser_PasswordMissingLowercase_FailureWithValidationError()
        {
            _identityConfig.Password.RequireLowercase = true;

            var result = await RegisterUserAsync(password: "UPPERCASE");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("password requires a lowercase character", StringComparison.OrdinalIgnoreCase)));
        }

        // Password Requires Uppercase
        [TestMethod]
        public async Task RegisterUser_PasswordContainsUppercase_Success()
        {
            _identityConfig.Password.RequireUppercase = true;
            var result = await RegisterUserAsync(password: "UPPERCASE");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public async Task RegisterUser_PasswordMissingUppercase_FailureWithValidationError()
        {
            _identityConfig.Password.RequireUppercase = true;
            var result = await RegisterUserAsync(password: "lowercase");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("password requires a uppercase character", StringComparison.OrdinalIgnoreCase)));
        }

        // Password Requires Non-Alphanumeric
        [TestMethod]
        public async Task RegisterUser_PasswordContainsNonAlphanumeric_Success()
        {
            _identityConfig.Password.RequireNonAlphanumeric = true;
            var result = await RegisterUserAsync(password: "Non-Alphanumeric");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public async Task RegisterUser_PasswordMissingNonAlphaNumeric_FailureWithValidationError()
        {
            _identityConfig.Password.RequireNonAlphanumeric = true;
            var result = await RegisterUserAsync(password: "NotNonAlphanumeric");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("password requires a non-alphanumeric character", StringComparison.OrdinalIgnoreCase)));
        }

        // Password Required Digit
        [TestMethod]
        public async Task RegisterUser_PasswordContainsDigit_Success()
        {
            _identityConfig.Password.RequireDigit = true;
            var result = await RegisterUserAsync(password: "G00D 71M35");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public async Task RegisterUser_PasswordMissingDigit_FailureWithValidationError()
        {
            _identityConfig.Password.RequireDigit = true;
            var result = await RegisterUserAsync(password: "Good Times");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("password requires a digit", StringComparison.OrdinalIgnoreCase)));
        }

        // Password Requires x Unique Characters
        [TestMethod]
        public async Task RegisterUser_PasswordMeetsUniqueCharacterRequirements_Success()
        {
            _identityConfig.Password.RequiredUniqueChars = 4;
            var result = await RegisterUserAsync(password: "abracadabra");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public async Task RegisterUser_PasswordTooFewUniqueCharacters_FailureWithValidationError()
        {
            _identityConfig.Password.RequiredUniqueChars = 4;
            var result = await RegisterUserAsync(password: "accadacca");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains($"password requires atleast {_identityConfig.Password.RequiredUniqueChars} unique characters", StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

        private Task<Result<RegisterResponse>> RegisterUserAsync(string email = "tester@gmail.com", string username = "AwesomeTester", string password = "8Toast")
        {
            var user = new RegistrationRequest
            {
                Email = email,
                UserName = username,
                Password = password,
            };

            return _userAccount.RegisterUserAsync(user);
        }
    }
}
