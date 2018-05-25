using OnlineSalesTool.Logic;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.AppEnum;
using System.Linq;
using OnlineSalesTool.Service;

namespace OnlineSalesTool.Service
{
    /// <summary>
    /// Shared base of all repos, common utilities of repo placed here
    /// </summary>
    public class ServiceBase : IService, IDisposable
    {
        public int UserId
        {
            get
            {
                //For testing without Auth
                //return 1; //Admin
                if (!TryGetContextValue<int>(UserPrincipal, CustomClaims.UserId, out int userId))
                    //Happens if no auth or no claim added
                    throw new InvalidOperationException($"Cant get {CustomClaims.UserId} of current acting principal");
                return userId;
            }
        }
        public string Username
        {
            get
            {
                //For testing without Auth
                //return 1; //Admin
                if (!TryGetContextValue<string>(UserPrincipal, CustomClaims.Username, out string name))
                    //Happens if no auth or no claim added
                    throw new InvalidOperationException($"Cant get {CustomClaims.Username} of current acting principal");
                return name;
            }
        }
        public RoleEnum Role
        {
            get
            {
                if (!TryGetContextValue<string>(UserPrincipal, CustomClaims.UserRole, out string role))
                    //Happens if no auth or no claim added
                    throw new InvalidOperationException($"Cant get {CustomClaims.UserRole} of current acting principal");
                return (RoleEnum)Enum.Parse(typeof(RoleEnum), role);
            }
        }
        //Not knowing what to include
        //protected AppUser User
        //{
        //    get
        //    {
        //        return DbContext.AppUser.First(u => u.UserId == UserId);
        //    }
        //}
        public ClaimsPrincipal UserPrincipal { get; private set; }

        public OnlineSalesContext DbContext { get; private set; }

        public ServiceBase(OnlineSalesContext context, IUserResolver userResolver) : this(userResolver.GetPrincipal(), context)
        {
            
        }

        public ServiceBase(ClaimsPrincipal principal, OnlineSalesContext context)
        {
            UserPrincipal = principal;
            DbContext = context;
        }
        private bool TryGetContextValue<T>(HttpContext context, string claimType, out T value)
        {
            if (context == null) throw new ArgumentNullException();
            return TryGetContextValue<T>(context.User, claimType, out value);
        }
        private bool TryGetContextValue<T>(ClaimsPrincipal principal, string claimType, out T value)
        {
            if (principal == null) throw new ArgumentNullException();
            value = default(T);
            var claim = principal.FindFirst(claimType);
            if (claim == null) return false;
            try
            {
                value = (T)Convert.ChangeType(claim.Value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        public void Dispose()
        {
            if (DbContext != null) DbContext.Dispose();
        }
    }
}
