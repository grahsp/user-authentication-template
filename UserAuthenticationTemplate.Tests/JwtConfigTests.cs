using UserAuthenticationTemplate.Configs;
namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class JwtConfigTests
    {
        [TestMethod]
        public void ValidateIssuer_ShouldDependOn_IssuerBeingSet()
        {
            var config = new JwtConfig
            {
                Issuer = ""
            };
            Assert.IsFalse(config.ValidateIssuer);
            config.Issuer = "localhost";
            Assert.IsTrue(config.ValidateIssuer);
        }

        [TestMethod]
        public void ValidateAudience_ShouldDependOn_AudienceBeingSet()
        {
            var config = new JwtConfig
            {
                Audience = ""
            };
            Assert.IsFalse(config.ValidateAudience);
            config.Audience = "localhost";
            Assert.IsTrue(config.ValidateAudience);
        }

        [TestMethod]
        public void ExpiresInMinutes_ShouldNotBeNegative_WhenSetToLessThanZero()
        {
            var config = new JwtConfig
            {
                ExpiresInMinutes = -27
            };

            Assert.IsTrue(config.ExpiresInMinutes == 0);
        }
    }
}