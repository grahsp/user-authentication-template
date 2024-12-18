using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Services
{
    public interface IUserService
    {
        Task<Result<RegisterResponse>> RegisterUserAsync(RegistrationRequest request);
        Task<Result<LoginResponse>> LoginUserAsync(LoginRequest request);
    }
}
