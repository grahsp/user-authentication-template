namespace UserAuthenticationTemplate.Configs.Identity
{
    public class LockoutConfig
    {
        public bool Enabled
        {
            get => _lockoutTimeSpan > TimeSpan.Zero && _attempts > 0;
        }

        public bool AllowedForNewUsers { get; set; } = true;

        private int _attempts = 0;
        public int MaxFailedAccessAttempts
        {
            get => _attempts;
            set => _attempts = value > 0 ? value : 0;
        }

        public int DefaultLockoutInMinutes
        {
            get => (int)Math.Ceiling(DefaultLockoutTimeSpan.TotalMinutes);
            set => DefaultLockoutTimeSpan = value > 0 ? TimeSpan.FromMinutes(value) : TimeSpan.Zero;
        }

        // Timespan method should be last in order to overwrite others (incase multiple lockout times are set)
        private TimeSpan _lockoutTimeSpan = TimeSpan.Zero;
        public TimeSpan DefaultLockoutTimeSpan
        {
            get => _lockoutTimeSpan;
            set => _lockoutTimeSpan = value > TimeSpan.Zero ? value : TimeSpan.Zero;
        }
    }
}
