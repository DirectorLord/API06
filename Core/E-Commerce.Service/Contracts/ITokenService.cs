using E_Commerce.Domain.Entities.Auth;

namespace E_Commerce.Service.Contracts;

public interface ITokenService
{
    string GetToken(ApplicationUser user, IList<string>roles);
}
