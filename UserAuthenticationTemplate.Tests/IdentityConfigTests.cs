using UserAuthenticationTemplate.Configs.Identity;

namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class IdentityConfigTests
    {
        #region Identity Tests
        [TestMethod]
        public void IdentityConfig_ShouldNotContainNullProperties()
        {
            var idConfig = new IdentityConfig();

            Assert.IsNotNull(idConfig.Lockout);
            Assert.IsNotNull(idConfig.Password);
            Assert.IsNotNull(idConfig.SignIn);
            Assert.IsNotNull(idConfig.User);
        }
        #endregion

        #region Lockout Tests
        [TestMethod]
        public void Lockout_MaxFailedAccessAttempts_IsSetToZero_WhenNegative()
        {
            var idConfig = new IdentityConfig();
            var attempts = -5;

            idConfig.Lockout.MaxFailedAccessAttempts = attempts;

            Assert.AreEqual(0, idConfig.Lockout.MaxFailedAccessAttempts,
                "MaxFailedAccessAttempts should be set to 0 when a negative value is provided.");
        }

        [TestMethod]
        public void Lockout_DefaultLockoutTimeSpan_IsSetToZero_WhenNegative()
        {
            var idConfig = new IdentityConfig();
            var minutes = -19;

            idConfig.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(minutes);

            Assert.AreEqual(TimeSpan.Zero, idConfig.Lockout.DefaultLockoutTimeSpan,
                "DefaultLockoutTimeSpan should be set to TimeSpan.Zero when a negative value is provided.");
        }

        [TestMethod]
        public void Lockout_DefaultLockoutInMinutes_IsSetToZero_WhenNegative()
        {
            var idConfig = new IdentityConfig();
            var minutes = -3;

            idConfig.Lockout.DefaultLockoutInMinutes = minutes;

            Assert.AreEqual(0, idConfig.Lockout.DefaultLockoutInMinutes,
                "DefaultLockoutInMinutes should be set to 0 when a negative value is provided.");
        }

        [TestMethod]
        public void Lockout_DefaultLockoutInMinutes_ShouldUpdate_DefaultLockoutTimeSpan()
        {
            var idConfig = new IdentityConfig();
            var minutes = 128;

            idConfig.Lockout.DefaultLockoutInMinutes = minutes;

            Assert.AreEqual(idConfig.Lockout.DefaultLockoutTimeSpan.TotalMinutes, idConfig.Lockout.DefaultLockoutInMinutes,
                "DefaultLockoutInMinutes should update the value of DefaultLockoutTimeSpan but in a TimeSpan format.");
        }

        [TestMethod]
        public void Lockout_DefaultLockoutTimeSpan_ShouldUpdate_DefaultLockoutInMinutes()
        {
            var idConfig = new IdentityConfig();
            var minutes = 721;

            idConfig.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(minutes);

            Assert.AreEqual(idConfig.Lockout.DefaultLockoutTimeSpan.TotalMinutes, idConfig.Lockout.DefaultLockoutInMinutes,
                "DefaultLockoutTimeSpan should update the value of DefaultLockoutInMinutes but in a int format.");
        }

        [TestMethod]
        public void Lockout_DefaultLockoutInMinutes_ShouldRoundUp()
        {
            var idConfig = new IdentityConfig();
            var minutes = 18;

            idConfig.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, minutes, 1);

            Assert.AreEqual(minutes + 1, idConfig.Lockout.DefaultLockoutInMinutes,
                "DefaultLockoutInMinutes should round up.");
        }

        [TestMethod]
        public void Lockout_Enabled_ShouldBeTrue_IfLockoutTime_AndMaxAttempts_IsGreaterThanZero()
        {
            var idConfig = new IdentityConfig();
            var attempts = 2;
            var minutes = 20;

            idConfig.Lockout.MaxFailedAccessAttempts = attempts;
            idConfig.Lockout.DefaultLockoutInMinutes = minutes;

            Assert.AreEqual(true, idConfig.Lockout.Enabled,
                "Enabled should be set to true if DefaultLockout and MaxFailedAccessAttempts are both greater than zero.");
        }

        [TestMethod]
        public void Lockout_Enabled_ShouldBeFalse_IfLockoutTime_IsZero()
        {
            var idConfig = new IdentityConfig();
            var attempts = 3;

            idConfig.Lockout.MaxFailedAccessAttempts = attempts;
            idConfig.Lockout.DefaultLockoutInMinutes = 0;

            Assert.AreEqual(false, idConfig.Lockout.Enabled,
                "Enabled should be set to false if DefaultLockout is zero.");
        }

        [TestMethod]
        public void Lockout_Enabled_ShouldBeFalse_IfMaxAttempts_IsZero()
        {
            var idConfig = new IdentityConfig();
            var minutes = 10;

            idConfig.Lockout.MaxFailedAccessAttempts = 0;
            idConfig.Lockout.DefaultLockoutInMinutes = minutes;

            Assert.AreEqual(false, idConfig.Lockout.Enabled,
                "Enabled should be set to false if DefaultLockout is zero.");
        }
        #endregion

        #region Password Tests
        [TestMethod]
        public void Password_RequiredLength_ShouldNotBeLessOrEqualZero()
        {
            var idConfig = new IdentityConfig();

            idConfig.Password.RequiredLength = 0;

            Assert.IsTrue(idConfig.Password.RequiredLength > 0,
                "RequiredLength should have a minimum value set greater than zero.");
        }

        [TestMethod]
        public void Password_RequiredUniqueChars_ShoulNotBeLessOrEqualZero()
        {
            var idConfig = new IdentityConfig();

            idConfig.Password.RequiredUniqueChars = 0;

            Assert.IsTrue(idConfig.Password.RequiredUniqueChars > 0,
                "RequiredUniqueChars should have a minimum value set greater than zero.");
        }

        [TestMethod]
        public void Password_RequiredLength_GreaterThan_RequiredUniqueChars()
        {
            var idConfig = new IdentityConfig();
            var minLength = 6;
            var minRequired = 79582;

            idConfig.Password.RequiredLength = minLength;
            idConfig.Password.RequiredUniqueChars = minRequired;

            Assert.IsTrue(idConfig.Password.RequiredLength >= idConfig.Password.RequiredUniqueChars,
                "RequiredLength should always be greater or equal to RequiredUniqueChars.");
            Assert.AreEqual(idConfig.Password.RequiredUniqueChars, minLength,
                "RequiredUniqueChars should be set to RequiredLength if too large a value was set.");
        }
        #endregion

        #region User Tests
        [TestMethod]
        public void UserConfig_AllowedUsernameCharacters_ShouldNotBeEmptyOrNull()
        {
            var idConfig = new IdentityConfig();

            Assert.IsFalse(string.IsNullOrEmpty(idConfig.User.AllowedUsernameCharacters),
                "AllowedUsernameCharacters should not be empty or null by default.");
        }
        #endregion
    }
}