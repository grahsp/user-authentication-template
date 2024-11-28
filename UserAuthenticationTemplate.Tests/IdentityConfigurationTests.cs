using UserAuthenticationTemplate.Configs.Identity;

namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class IdentityConfigurationTests
    {
        #region General Tests
        [TestMethod]
        public void IdentityConfig_ShouldNotContainNullProperties()
        {
            var idConfig = new IdentityConfig();

            Assert.IsNotNull(idConfig.Lockout);
            Assert.IsNotNull(idConfig.Password);
            Assert.IsNotNull(idConfig.SignIn);
            Assert.IsNotNull(idConfig.User);
        }

        [TestMethod]
        public void UserConfig_AllowedUsernameCharacters_ShouldNotBeEmptyOrNull()
        {
            var idConfig = new IdentityConfig();

            Assert.IsFalse(string.IsNullOrEmpty(idConfig.User.AllowedUsernameCharacters));
        }
        #endregion
    }
}