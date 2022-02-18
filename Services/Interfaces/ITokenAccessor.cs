using Models;

namespace Services.Interfaces;

public interface ITokenAccessor
{
    string CreateToken(AppUser user);
}
