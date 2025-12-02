using PetSearchHome.DAL.Domain.Entities;

namespace PetSearchHome.BLL.Services.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(RegisteredUser user);
}
