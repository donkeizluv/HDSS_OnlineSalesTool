// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using OnlineSalesCore.Const;
// using OnlineSalesCore.Models;

// namespace OnlineSalesCore.Cache
// {
//     public class MailListCache : IMailListCache
//     {
//         ILogger _logger;
//         public MailListCache(OnlineSalesContext context, ILogger<MailListCache> logger)
//         {
//             _logger = logger;
//             // Populate cache
//             if(_bds == null)
//             {
//                 _bds = context.AppUser
//                     .Where(u => u.Role.Name == RoleEnum.BDS.ToString())
//                     .ToDictionary(k => k.UserId, v => v.Email);
//             }
//             if(_admin == null)
//             {
//                 _admin = context.AppUser
//                     .Where(u => u.Role.Name == RoleEnum.ADMIN.ToString())
//                     .ToDictionary(k => k.UserId, v => v.Email);
//             }
//         }
//         private static IDictionary<int, string> _bds;
//         private static IDictionary<int, string> _admin;

//         public IDictionary<int, string> BDSEmails => _bds;

//         public IDictionary<int, string> AdminEmails => _admin;

//         public void AddBDSMail(int id, string email)
//         {
//             if(!_bds.TryAdd(id, email))
//             {
//                 _logger.LogError($"Cache new BDS email failed. id: {id} email: {email}");
//             }
//         }

//         public void AddAdminMail(int id, string email)
//         {
//             if(!_admin.TryAdd(id, email))
//             {
//                 _logger.LogError($"Cache new ADMIN email failed. id: {id} email: {email}");
//             }
//         }
//     }
// }
