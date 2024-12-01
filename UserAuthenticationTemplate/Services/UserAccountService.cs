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
    }
}
