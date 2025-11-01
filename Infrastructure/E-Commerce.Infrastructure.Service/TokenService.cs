using E_Commerce.Domain.Entities.Auth;
using E_Commerce.Service.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Infrastructure.Service;

public class TokenService
    : ITokenService
{
    public string GetToken(ApplicationUser user, IList<string> roles)
    {
        List<Claim> claims = [
            new(JwtRegisteredClaimNames.Name, user.DisplayName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            ];
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes("This is my custom Secret key for authentication"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(claims: claims, issuer: "MyApplication", audience: "MyApplication", expires: DateTime.Now.AddDays(2), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
