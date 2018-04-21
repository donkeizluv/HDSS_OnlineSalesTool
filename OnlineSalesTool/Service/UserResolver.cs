using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OnlineSalesTool.Service
{
    public class UserResolver : IUserResolver
    {
        private readonly IHttpContextAccessor _context;
        public UserResolver(IHttpContextAccessor context)
        {
            _context = context;
        }

        public ClaimsPrincipal GetPrincipal()
        {
            return _context.HttpContext.User;
        }
    }
}
