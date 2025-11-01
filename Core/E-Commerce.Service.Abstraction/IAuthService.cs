using E_Commerce.Shared.DataTransferObjects.Auth;

namespace E_Commerce.Service.Abstraction;

public interface IAuthService
{
    Task<UserResponse> LoginAsync(LoginRequest loginRequest);
    Task<UserResponse> RegisterAsync(RegisterRequest registerRequest);
}
