using System;
using System.Collections.Generic;

namespace OnlineSalesCore.Models
{
    public partial class UserAbility
    {
        public int UserId { get; set; }
        public int AbilityId { get; set; }

        public Ability Ability { get; set; }
        public AppUser User { get; set; }
    }
}
