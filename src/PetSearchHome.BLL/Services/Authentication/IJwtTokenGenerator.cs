using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Services.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(RegisteredUser user);
}
