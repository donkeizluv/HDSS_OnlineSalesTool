using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.POCO
{
    public class AppUserPOCO
    {
        private string _username;

        public AppUserPOCO()
        {

        }
        public AppUserPOCO(AppUser user)
        {
            UserId = user.UserId;
            Name = user.Name;
            Username = user.Username.ToLower();
            HR = user.Hr;
            Phone = user.Phone;
            Phone2 = user.Phone2;
            Active = user.Active;
        }
        public int UserId { get; set; }
        public string DisplayName => $"{Username} - {HR}";
        public string Name { get; set; }
        public string Username { get => _username; set => _username = value.ToLower(); }
        public string HR { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Role { get; set; }
        public AppUserPOCO Manager { get; set; }
        public bool Active { get; set; }
    }
}
