using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class Ability
    {
        public Ability()
        {
            UserAbility = new HashSet<UserAbility>();
        }

        public int AbilityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<UserAbility> UserAbility { get; set; }
    }
}
