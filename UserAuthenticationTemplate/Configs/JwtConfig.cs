namespace UserAuthenticationTemplate.Configs
{
    public class JwtConfig
    {
        public string? Secret { get; set; }
        private string _issuer = "";
        public bool ValidateIssuer { get; private set; }
        public string Issuer
        {
            get => _issuer;
            set
            {
                _issuer = value;
                ValidateIssuer = !string.IsNullOrEmpty(_issuer);
            }
        }
        private string _audience = "";
        public bool ValidateAudience { get; private set; }
        public string Audience
        {
            get => _audience;
            set
            {
                _audience = value;
                ValidateAudience = !string.IsNullOrEmpty(_audience);
            }
        }

        private int _expiresInMinutes = 30;
        public int ExpiresInMinutes
        {
            get => _expiresInMinutes;
            set => _expiresInMinutes = value > 0 ? value : 0;
        }

        public DateTime Expires
        {
            get => DateTime.Now + TimeSpan.FromMinutes(_expiresInMinutes);
        }

        public int ClockSkewInMinutes
        {
            get => (int)Math.Ceiling(ClockSkew.TotalMinutes);
            set => ClockSkew = TimeSpan.FromMinutes(value);
        }
        private TimeSpan _clockSkew = TimeSpan.FromMinutes(5);
        public TimeSpan ClockSkew
        {
            get => _clockSkew;
            set => _clockSkew = value > TimeSpan.Zero ? value : TimeSpan.Zero;
        }
    }
}