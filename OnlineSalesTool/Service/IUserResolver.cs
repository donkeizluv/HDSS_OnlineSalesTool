using System.Security.Claims;

namespace OnlineSalesTool.Service
{
    public interface IUserResolver
    {
        ClaimsPrincipal GetPrincipal();
    }
}
