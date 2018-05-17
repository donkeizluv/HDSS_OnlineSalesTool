using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.POCO
{
    public class AppUserPOCO
    {
        public AppUserPOCO()
        {

        }
        public AppUserPOCO(AppUser user)
        {
            UserId = user.UserId;
            DisplayName = $"{user.Username} - {user.Hr}";
        }
        public int UserId { get; set; }
        //public string Username { get; set; }
        public string DisplayName { get; set; }
    }
}
