using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserAuthenticationTemplate.Configs.Identity;
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
    }
}
