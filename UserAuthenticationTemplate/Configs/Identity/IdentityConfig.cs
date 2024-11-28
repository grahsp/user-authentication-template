namespace UserAuthenticationTemplate.Configs.Identity
{
    public class IdentityConfig
    {
        public PasswordConfig Password { get; set; } = new PasswordConfig();
        public LockoutConfig Lockout { get; set; } = new LockoutConfig();
        public SignInConfig SignIn { get; set; } = new SignInConfig();
        public UserConfig User { get; set; } = new UserConfig();
    }
}
