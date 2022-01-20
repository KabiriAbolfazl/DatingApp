using WebApi.Entities;

namespace WebApi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser);
    }
}
