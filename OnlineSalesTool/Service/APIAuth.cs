using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineSalesTool.DTO;
using static OnlineSalesTool.Controllers.AccountController;
using Microsoft.Extensions.Options;
using OnlineSalesTool.Options;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

namespace OnlineSalesTool.Const
{
    public class APIAuth : IAPIAuth
    {
        private readonly APIAuthOptions _options;
        public APIAuth(IOptions<APIAuthOptions> options) => _options = options.Value ?? throw new ArgumentNullException();
        public bool Check(string sig, string guid)
        {
            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(sig))
            {
                throw new ArgumentException();
            }
            var forge = Hash256(guid + _options.Pwd);
            return string.Compare(sig, forge, false, CultureInfo.InvariantCulture) == 0;
        }
        public string Forge(string guid)
        {
            if (guid == null)
            {
                throw new ArgumentNullException(nameof(guid));
            }
            return Hash256(guid + _options.Pwd);
        }
        private static string Hash256(string value)
        {  
            var sb = new StringBuilder();
            using (var hash = SHA256.Create())            
            {
                var result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }  
    }
}
