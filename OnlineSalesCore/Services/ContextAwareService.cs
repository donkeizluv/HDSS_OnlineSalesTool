﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineSalesCore.Const;
using OnlineSalesCore.Models;
using OnlineSalesCore.Exceptions;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    /// <summary>
    /// Shared base of all services, common utilities of service placed here
    /// </summary>
    public class ContextAwareService : IContextAwareService
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
        public string Email
        {
            get
            {
                //For testing without Auth
                //return 1; //Admin
                if (!TryGetContextValue<string>(UserPrincipal, CustomClaims.Email, out string email))
                    //Happens if no auth or no claim added
                    throw new InvalidOperationException($"Cant get {CustomClaims.Email} of current acting principal");
                return email;
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
        public ClaimsPrincipal UserPrincipal { get; private set; }

        public OnlineSalesContext DbContext { get; private set; }
        public ContextAwareService(IHttpContextAccessor httpContext, OnlineSalesContext context) : this(httpContext.HttpContext.User, context)
        {

        }
        protected ContextAwareService(ClaimsPrincipal principal, OnlineSalesContext context)
        {
            UserPrincipal = principal ?? throw new ArgumentNullException();
            DbContext = context ?? throw new ArgumentNullException();
        }
        /// <summary>
        /// Check if user id match the role
        /// </summary>
        /// <param name="userId">id to check</param>
        /// <param name="role">target role</param>
        /// <param name="throwOnNullId">throw on null id</param>
        /// <returns></returns>
        protected virtual async Task CheckUser(int? userId, RoleEnum role, bool throwOnNullId = false)
        {
            if (throwOnNullId)
            {
                if (userId == null) throw new BussinessException($"Null user for role: {role.ToString()}");
            }
            if (!await DbContext.AppUser
                .AnyAsync(u => u.UserId == userId && u.Role.Name == role.ToString()))
                throw new BussinessException($"User id: {userId} is not exist or not in valid role: {role.ToString()}");
        }

        private bool TryGetContextValue<T>(HttpContext context, string claimType, out T value)
        {
            if (context == null) throw new ArgumentNullException();
            return TryGetContextValue<T>(context.User, claimType, out value);
        }
        private bool TryGetContextValue<T>(ClaimsPrincipal principal, string claimType, out T value)
        {
            if (principal == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(claimType)) throw new ArgumentNullException();
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
            if (DbContext != null)
                DbContext.Dispose();
        }
    }
}
