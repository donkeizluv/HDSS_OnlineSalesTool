using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class UserRole
    {
        public UserRole()
        {
            AppUser = new HashSet<AppUser>();
        }

        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<AppUser> AppUser { get; set; }
    }
}
