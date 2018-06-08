using Newtonsoft.Json;
using OnlineSalesCore.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineSalesTool.AuthToken
{
    public class Token
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity,
            IJwtFactory jwtFactory,
            string userName,
            JwtIssuerOptions jwtOptions,
            JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };
            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
