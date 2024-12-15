namespace UserAuthenticationTemplate.Configs
{
    /// <summary>
    /// Configuration settings for JSON Web Tokens.
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// The secret key used to sign JWT tokens.
        /// </summary>
        /// <remarks>
        /// This value should be a secure, randomly generated string and must remain confidential.
        /// </remarks>
        public string? Secret { get; set; }


        private string _issuer = "";

        /// <summary>
        /// Indicates whether the issuer of the token should be validated.
        /// </summary>
        /// <remarks>
        /// This property is automatically set to <see langword="true"/> if <see cref="Issuer"/> has a non-empty value.
        /// </remarks>
        public bool ValidateIssuer { get; private set; }

        /// <summary>
        /// The issuer of the JWT tokens.
        /// </summary>
        /// <remarks>
        /// This is typically the name or URL of the application issuing the tokens. 
        /// Setting this property will automatically update <see cref="ValidateIssuer"/>.
        /// </remarks>
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

        /// <summary>
        /// Indicates whether the audience of the token should be validated.
        /// </summary>
        /// <remarks>
        /// This property is automatically set to <see langword="true"/> if <see cref="Audience"/> has a non-empty value.
        /// </remarks>
        public bool ValidateAudience { get; private set; }

        /// <summary>
        /// The intended audience of the JWT tokens.
        /// </summary>
        /// <remarks>
        /// This is typically the name or URL of the target application the token is intended for.
        /// Setting this property will automatically update <see cref="ValidateAudience"/>.
        /// </remarks>
        public string Audience
        {
            get => _audience;
            set
            {
                _audience = value;
                ValidateAudience = !string.IsNullOrEmpty(_audience);
            }
        }


        private int _expiresInMinutes = 20;

        /// <summary>
        /// The expiration time for the token in minutes.
        /// </summary>
        /// <remarks>
        /// Defaults to 30 minutes. If a value less than or equal to 0 is set, it will be treated as 0.
        /// </remarks>
        public int ExpiresInMinutes
        {
            get => _expiresInMinutes;
            set => _expiresInMinutes = value > 0 ? value : 0;
        }

        /// <summary>
        /// The exact expiration time of the token based on the current time and <see cref="ExpiresInMinutes"/>.
        /// </summary>
        /// <remarks>
        /// This property is read-only and dynamically calculates the expiration time.
        /// </remarks>
        public DateTime Expires
        {
            get => DateTime.Now + TimeSpan.FromMinutes(_expiresInMinutes);
        }


        private TimeSpan _clockSkew = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The allowable clock skew for token validation as a <see cref="TimeSpan"/>.
        /// </summary>
        /// <remarks>
        /// This property is useful for handling minor time discrepancies between the server and clients. Defaults to 5 minutes.
        /// </remarks>
        public TimeSpan ClockSkew
        {
            get => _clockSkew;
            set => _clockSkew = value > TimeSpan.Zero ? value : TimeSpan.Zero;
        }

        /// <summary>
        /// The allowable clock skew for token validation in minutes.
        /// </summary>
        /// <remarks>
        /// Defaults to 5 minutes. If a value less than or equal to 0 is set, it will be treated as 0.
        /// </remarks>
        public int ClockSkewInMinutes
        {
            get => (int)Math.Ceiling(ClockSkew.TotalMinutes);
            set => ClockSkew = TimeSpan.FromMinutes(value);
        }
    }
}