namespace UserAuthenticationTemplate.Configs.Identity
{
    public class PasswordConfig
    {
        public bool RequireDigit { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireNonAlphanumeric { get; set; } = false;

        private int _length = 6;
        private readonly int _lengthMin = 4;
        public int RequiredLength
        {
            get => _length;
            set => _length = value > _lengthMin ? value : _lengthMin;
        }

        private int _unique = 2;
        private readonly int _uniqueMin = 2;
        public int RequiredUniqueChars
        {
            get => _unique;
            // _unique has to be less than _lengthMin since you can't have
            // more unique characters than the passwords length
            set => _unique = Math.Clamp(value, _uniqueMin, _length);
        }
    }
}
