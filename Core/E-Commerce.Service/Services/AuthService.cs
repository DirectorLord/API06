using E_Commerce.Domain.Entities.Auth;
using E_Commerce.Service.Abstraction.Common;
using E_Commerce.Shared.DataTransferObjects.Auth;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Service.Services;

public class AuthService(UserManager<ApplicationUser> userManager)
    : IAuthService
{
    public async Task<Result<UserResponse>> LoginAsync(LoginRequest loginRequest)
    {
        var user = await userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null) 
            return Error.Unauthorized(description: "Invalid email or password");
        var result = await userManager.CheckPasswordAsync(user, loginRequest.Password);
        if (!result) 
            return Error.Unauthorized(description: "Invalid email or password");
        return new UserResponse(user.Email, user.DisplayName, "Token");
    }

    public async Task<Result<UserResponse>> RegisterAsync(RegisterRequest registerRequest)
    {
        var user = new ApplicationUser
        {
            Email = registerRequest.Email,
            UserName = registerRequest.UserName,
            DisplayName = registerRequest.DisplayName,
            PhoneNumber = registerRequest.PhoneNumber
        };
        var result = await userManager.CreateAsync(user, registerRequest.Password);
        if(result.Succeeded) return new UserResponse(user.Email, user.DisplayName, "Token");

        return result.Errors.Select(e => Error.Validation(e.Code, e.Description))
            .ToList();
    }
}
