using Entities.Domain;

namespace UseCases.API.Core
{
    public interface IJwtService
    {
        string CreateToken(ApplicationUser applicationUser);
    }
}
