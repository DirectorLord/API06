using E_Commerce.Service.Abstraction;
using E_Commerce.Shared.DataTransferObjects.Auth;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.API.Controllers;

public class AuthController(IAuthService auth)
    : APIBaseController
{
    [HttpPost("Register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var result = await auth.RegisterAsync(request);
        return HandleResult(result);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        var result = await auth.LoginAsync(request);
        return HandleResult(result);
    }
}
