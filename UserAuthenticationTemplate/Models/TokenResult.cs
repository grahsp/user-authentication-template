using UserAuthenticationTemplate.Shared.Enums;

namespace UserAuthenticationTemplate.Models
{
    public abstract class ResultBase<TErrorEnum> where TErrorEnum : Enum
    {
        public bool IsSuccess { get; protected set; }
        public bool IsFailure { get => !IsSuccess; }
        public TErrorEnum? Code { get; protected set; }
        public string[] ErrorMessages { get; protected set; } = [];
    }

    public class TokenValidationResult : ResultBase<SecurityError>
    {
        public static TokenValidationResult Success()
        {
            return new() { IsSuccess = true };
        }

        public static TokenValidationResult Failure(SecurityError code, params string[] errorMessage)
        {
            return new() {
                IsSuccess = false,
                ErrorMessages = errorMessage,
                Code = code
            };
        }
    }

    public class TokenResult : ResultBase<SecurityError>
    {
        private string? _token;
        public string Token
        {
            get => IsSuccess && !string.IsNullOrEmpty(_token) ? _token : throw new InvalidOperationException("No token exists because the operation failed.");
            set => _token = value;
        }

        public static TokenResult Success(string token)
        {
            return new() {
                IsSuccess = true,
                Token = token
            };
        }

        public static TokenResult Failure(SecurityError code, params string[] errorMessage)
        {
            return new() { 
                IsSuccess = false,
                ErrorMessages = errorMessage,
                Code = code
            };
        }
    }
}
