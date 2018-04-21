using OnlineSalesTool.Service;
using OnlineSalesTool.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    /// <summary>
    /// Shared base of all repos, common utilities of repo placed here
    /// </summary>
    public abstract class BaseRepo
    {
        public virtual int PrincipalUserId
        {
            get
            {
#if DEBUG
                return 1; //Admin
#endif
                if (!ContextHelper.TryGetContextValue<int>(UserPrincipal, CustomClaims.UserId, out int userId))
                    //Cant happen if user has signed in & proper claims added
                    throw new InvalidOperationException("Cant get UserId of current acting principal");
                return userId;
            }
        }
        //Audit user permission
        public ClaimsPrincipal UserPrincipal { get; private set; }
        public BaseRepo(ClaimsPrincipal principal)
        {
            UserPrincipal = principal;
        }
    }
}
