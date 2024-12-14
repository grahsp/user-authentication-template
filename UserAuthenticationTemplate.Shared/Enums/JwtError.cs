namespace UserAuthenticationTemplate.Shared.Enums
{
    public enum JwtError
    {
        InvalidToken,
        InvalidAudience,
        InvalidIssuer,
        ExpiredToken,
        MissingToken,
        InvalidClaim,
        SignatureValidationFailed,
        MalformedToken,
        UnknownError,
    }
}
