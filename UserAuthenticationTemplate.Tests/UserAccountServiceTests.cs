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
                },
                Lockout = new LockoutConfig
                {
                    AllowedForNewUsers = true,
                    DefaultLockoutInMinutes = 1,
                    MaxFailedAccessAttempts = 3
                }
            };

            var userManager = new MockUserManager(_identityConfig);
            _logger = new MockLogger<UserAccountService>();
            _userAccount = new(userManager, _logger, Options.Create(_identityConfig));
        }

        #region Email Validation Tests
        // FIX: basic data validation only checks for '@'. Implement a more robust validation for email.
        [TestMethod]
        public async Task RegisterUser_EmailValid_Success()
        {
            var result = await RegisterUserAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public async Task RegisterUser_EmailMissingAt_FailureWithValidationError()
        {
            var result = await RegisterUserAsync(email: "testgmail.com");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("invalid email address", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_EmailMultipleAt_FailureWithValidationError()
        {
            var result = await RegisterUserAsync(email: "test@@gmail.com");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("invalid email address", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_EmailMissingDot_FailureWithValidationError()
        {
            var result = await RegisterUserAsync(email: "test@gmailcom");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("invalid email address", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_EmailContainInvalidCharacter_FailureWithValidationError()
        {
            var result = await RegisterUserAsync(email: "test!@gmail.com");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("invalid email address", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_EmailStartWithAt_FailureWithValidationError()
        {
            var result = await RegisterUserAsync(email: "@gmail.com");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("invalid email address", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_EmailAtFollowbedByDot_FailureWithValidationError()
        {
            var result = await RegisterUserAsync(email: "test@.com");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("invalid email address", StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

        #region Password Validation Tests
        // Password Required Length
        [TestMethod]
        public async Task RegisterUser_PasswordTooLong_FailureWithValidationResult()
        {
            var result = await RegisterUserAsync(password: new string('a', 255) + 'b');

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("password too long", StringComparison.OrdinalIgnoreCase)));
        }

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

        #region Invalid Request Data Tests
        [TestMethod]
        public async Task RegisterUser_UserEmailAndUsernameAlreadyExists_FailureWithValidationError()
        {
            // Result1
            var result1 = await RegisterUserAsync();
            Assert.IsTrue(result1.IsSuccess);
            Assert.IsFalse(result1.HasErrors);

            var result2 = await RegisterUserAsync();
            Assert.IsTrue(result2.IsFailure);
            Assert.IsTrue(result2.HasErrors);
            Assert.IsTrue(result2.Errors.Any(e => e.Contains("user already exists", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_UserEmailAlreadyExists_FailureWithValidationError()
        {
            // Result1
            var result1 = await RegisterUserAsync(username: null);
            Assert.IsTrue(result1.IsSuccess);
            Assert.IsFalse(result1.HasErrors);

            var result2 = await RegisterUserAsync(username: null);
            Assert.IsTrue(result2.IsFailure);
            Assert.IsTrue(result2.HasErrors);
            Assert.IsTrue(result2.Errors.Any(e => e.Contains("user already exists", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_UserUsernameAlreadyExists_FailureWithValidationError()
        {
            // Result1
            var result1 = await RegisterUserAsync(email: null);
            Assert.IsTrue(result1.IsSuccess);
            Assert.IsFalse(result1.HasErrors);

            var result2 = await RegisterUserAsync(email: null);
            Assert.IsTrue(result2.IsFailure);
            Assert.IsTrue(result2.HasErrors);
            Assert.IsTrue(result2.Errors.Any(e => e.Contains("user already exists", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_EmailDifferentCapitalization_FailureWithValidationError()
        {
            // Result1
            var result1 = await RegisterUserAsync(username: null, email: "TEST@GMAIL.COM");
            Assert.IsTrue(result1.IsSuccess);
            Assert.IsFalse(result1.HasErrors);

            var result2 = await RegisterUserAsync(username: null, email: "test@gmail.com");
            Assert.IsTrue(result2.IsFailure);
            Assert.IsTrue(result2.HasErrors);
            Assert.IsTrue(result2.Errors.Any(e => e.Contains("user already exists", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_UsernameDifferentCapitalization_FailureWithValidationError()
        {
            // Result1
            var result1 = await RegisterUserAsync(username: "testuser", email: null);
            Assert.IsTrue(result1.IsSuccess);
            Assert.IsFalse(result1.HasErrors);

            var result2 = await RegisterUserAsync(username: "TESTUSER", email: null);
            Assert.IsTrue(result2.IsFailure);
            Assert.IsTrue(result2.HasErrors);
            Assert.IsTrue(result2.Errors.Any(e => e.Contains("user already exists", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_MissingEmailAndUsername_FaillureWithValidationError()
        {
            var result = await RegisterUserAsync(email: null, username: null);

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("provide a valid email or username", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_UsernameExceedsMaximumLength_FailureWithValidationError()
        {
            var longUsername = new string('a', 256);

            var result = await RegisterUserAsync(username: longUsername);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("username too long", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task RegisterUser_EmailExceedsMaximumLength_FailureWithValidationError()
        {
            var longEmail = new string('a', 256) + "@example.com";

            var result = await RegisterUserAsync(email: longEmail);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("email too long", StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

        #region LoginUser Request Data Tests
        [TestMethod]
        public async Task LoginUser_WithValidRequest_Success()
        {
            _ = await RegisterUserAsync();

            var loginResult = await LoginUserAsync();

            Assert.IsTrue(loginResult.IsSuccess);
            Assert.IsFalse(loginResult.HasErrors);
        }

        [TestMethod]
        public async Task LoginUser_WithCapitalizedEmail_Success()
        {
            _ = await RegisterUserAsync(email: "tester@gmail.com");

            var loginResult = await LoginUserAsync(email: "TESTER@GMAIL.COM");

            Assert.IsTrue(loginResult.IsSuccess);
            Assert.IsFalse(loginResult.HasErrors);
        }

        [TestMethod]
        public async Task LoginUser_WithCapitalizedUsername_Success()
        {
            _ = await RegisterUserAsync(username: "AwesomeTester");

            var loginResult = await LoginUserAsync(username: "AWESOMETESTER");

            Assert.IsTrue(loginResult.IsSuccess);
            Assert.IsFalse(loginResult.HasErrors);
        }

        [TestMethod]
        public async Task LoginUser_WithUnregisteredUser_FailureWithError()
        {
            var loginResult = await LoginUserAsync();

            Assert.IsTrue(loginResult.IsFailure);
            Assert.IsTrue(loginResult.HasErrors);
            Assert.IsTrue(loginResult.Errors.Any(e => e.Contains("could not find user", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task LoginUser_InvalidPassword_FailureWithError()
        {
            _ = await RegisterUserAsync(password: "test");

            var loginResult = await LoginUserAsync(password: "tes");

            Assert.IsTrue(loginResult.IsFailure);
            Assert.IsTrue(loginResult.HasErrors);
            Assert.IsTrue(loginResult.Errors.Any(e => e.Contains("invalid password", StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

        #region User Lockout Tests
        [TestMethod]
        public async Task LoginUser_CanLoginAfterFailedAttempts_Success()
        {
            _identityConfig.Lockout.DefaultLockoutInMinutes = 5;
            _identityConfig.Lockout.MaxFailedAccessAttempts = 3;
            _ = await RegisterUserAsync(password: "test");

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts - 1; i++)
            {
                var loginResult1 = await LoginUserAsync(password: "tes");
                Assert.IsTrue(loginResult1.IsFailure);
                Assert.IsTrue(loginResult1.HasErrors);
                Assert.IsTrue(loginResult1.Errors.Any(e => e.Contains("invalid password", StringComparison.OrdinalIgnoreCase)));
            }

            var loginResult2 = await LoginUserAsync(password: "test");
            Assert.IsTrue(loginResult2.IsSuccess);
            Assert.IsFalse(loginResult2.HasErrors);
        }

        [TestMethod]
        public async Task LoginUser_LockoutAfterFailedLoginAttempts_FailureWithError()
        {
            _identityConfig.Lockout.DefaultLockoutInMinutes = 5;
            _identityConfig.Lockout.MaxFailedAccessAttempts = 2;
            _ = await RegisterUserAsync(password: "test");

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts; i++)
            {
                var loginResult1 = await LoginUserAsync(password: "tes");
                Assert.IsTrue(loginResult1.IsFailure);
                Assert.IsTrue(loginResult1.HasErrors);
                Assert.IsTrue(loginResult1.Errors.Any(e => e.Contains("invalid password", StringComparison.OrdinalIgnoreCase)));
            }

            var loginResult2 = await LoginUserAsync(password: "test");
            Assert.IsTrue(loginResult2.IsFailure);
            Assert.IsTrue(loginResult2.HasErrors);
            Assert.IsTrue(loginResult2.Errors.Any(e => e.Contains("user has been temporarily locked out", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task LoginUser_FailedAccessCountResetOnSuccessfulLogin_Success()
        {
            _identityConfig.Lockout.DefaultLockoutInMinutes = 5;
            _identityConfig.Lockout.MaxFailedAccessAttempts = 2;
            _ = await RegisterUserAsync(password: "test");

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < _identityConfig.Lockout.MaxFailedAccessAttempts - 1; j++)
                {
                    var loginResult1 = await LoginUserAsync(password: "tes");
                    Assert.IsTrue(loginResult1.IsFailure);
                    Assert.IsTrue(loginResult1.HasErrors);
                    Assert.IsTrue(loginResult1.Errors.Any(e => e.Contains("invalid password", StringComparison.OrdinalIgnoreCase)));
                }

                var loginResult2 = await LoginUserAsync(password: "test");
                Assert.IsTrue(loginResult2.IsSuccess);
                Assert.IsFalse(loginResult2.HasErrors);
            }
        }

        [TestMethod]
        public async Task LoginUser_LockoutShouldExpireAndResetAfterFailedAccess_Success()
        {
            _identityConfig.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(500);
            _identityConfig.Lockout.MaxFailedAccessAttempts = 2;
            _ = await RegisterUserAsync(password: "test");

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts + 1; i++)
            {
                var loginResult1 = await LoginUserAsync(password: "tes");
                Assert.IsTrue(loginResult1.IsFailure);
                Assert.IsTrue(loginResult1.HasErrors);
            }

            Thread.Sleep(_identityConfig.Lockout.DefaultLockoutTimeSpan);

            var loginResult2 = await LoginUserAsync(password: "test");
            Assert.IsTrue(loginResult2.IsSuccess);
            Assert.IsFalse(loginResult2.HasErrors);
        }

        [TestMethod]
        public async Task LoginUser_LockoutDoesNotAffectOtherUsers_Success()
        {
            _identityConfig.Lockout.DefaultLockoutInMinutes = 5;
            _identityConfig.Lockout.MaxFailedAccessAttempts = 2;
            _ = await RegisterUserAsync(password: "test");
            _ = await RegisterUserAsync(email: "otherTester@gmail.com", username: "BadTester", password: "test");

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts; i++)
            {
                var loginResult1 = await LoginUserAsync(password: "tes");
                Assert.IsTrue(loginResult1.IsFailure);
                Assert.IsTrue(loginResult1.HasErrors);
                Assert.IsTrue(loginResult1.Errors.Any(e => e.Contains("invalid password", StringComparison.OrdinalIgnoreCase)));
            }

            var loginResult2 = await LoginUserAsync(email: "otherTester@gmail.com", password: "test");
            Assert.IsTrue(loginResult2.IsSuccess);
            Assert.IsFalse(loginResult2.HasErrors);
        }

        [TestMethod]
        public async Task LoginUser_NoLockoutIfDisabled_Success()
        {
            _identityConfig.Lockout.DefaultLockoutInMinutes = 0;
            _identityConfig.Lockout.MaxFailedAccessAttempts = 0;
            _ = await RegisterUserAsync(password: "test");

            for (var i = 0; i < _identityConfig.Lockout.MaxFailedAccessAttempts + 1; i++)
            {
                var loginResult1 = await LoginUserAsync(password: "tes");
                Assert.IsTrue(loginResult1.IsFailure);
                Assert.IsTrue(loginResult1.HasErrors);
                Assert.IsTrue(loginResult1.Errors.Any(e => e.Contains("invalid password", StringComparison.OrdinalIgnoreCase)));
            }

            var loginResult2 = await LoginUserAsync(password: "test");
            Assert.IsTrue(loginResult2.IsSuccess);
            Assert.IsFalse(loginResult2.HasErrors);
        }
        #endregion

        private Task<Result<RegisterResponse>> RegisterUserAsync(string? email = "tester@gmail.com", string? username = "AwesomeTester", string password = "8Toast")
        {
            var user = new RegistrationRequest
            {
                Email = email,
                UserName = username,
                Password = password,
            };

            return _userAccount.RegisterUserAsync(user);
        }

        private Task<Result<LoginResponse>> LoginUserAsync(string? email = "tester@gmail.com", string? username = "AwesomeTester", string password = "8Toast")
        {
            var user = new LoginRequest
            {
                Email = email,
                UserName = username,
                Password = password
            };

            return _userAccount.LoginUserAsync(user);
        }
    }
}
