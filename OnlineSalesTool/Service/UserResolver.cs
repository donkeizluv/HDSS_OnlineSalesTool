using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OnlineSalesTool.Service
{
    public class UserResolver : IUserResolver
    {
        private readonly IHttpContextAccessor _httpContext;
        public UserResolver(IHttpContextAccessor context)
        {
            _httpContext = context;
        }

        public ClaimsPrincipal GetPrincipal()
        {
            return _httpContext.HttpContext.User;
        }
    }
}
