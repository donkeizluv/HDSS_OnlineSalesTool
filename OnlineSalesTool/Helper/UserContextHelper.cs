using Microsoft.AspNetCore.Http;
using OnlineSalesTool.Logic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineSalesTool.Service
{
    public static class ContextHelper
    {
        public static bool TryGetContextValue<T>(HttpContext context, CustomClaims type, out T value)
        {
            if(context == null) throw new ArgumentNullException();
            return TryGetContextValue<T>(context.User, type, out value);
        }
        public static bool TryGetContextValue<T>(ClaimsPrincipal principal, CustomClaims type, out T value)
        {
            if(principal == null) throw new ArgumentNullException();
            value = default(T);
            var claim = principal.FindFirst(type.ToString());
            if (claim == null) return false;
            try
            {
                value = (T)Convert.ChangeType(claim.Value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch(FormatException)
            {
                return false;
            }
            return true;
        }

        public static Dictionary<string, List<string>> ClaimsToDict(IEnumerable<Claim> claims)
        {
            if (claims == null) throw new ArgumentNullException();

            var dict = new Dictionary<string, List<string>>();
            foreach (var claim in claims)
            {
                if (dict.ContainsKey(claim.Type))
                    dict[claim.Type].Add(claim.Value);
                else
                    dict.Add(claim.Type, new List<string>() { claim.Value });
            }
            return dict;
        }
    }
}
