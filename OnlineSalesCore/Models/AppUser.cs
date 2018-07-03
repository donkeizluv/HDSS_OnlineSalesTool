using System;
using System.Collections.Generic;

namespace OnlineSalesCore.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            InverseManager = new HashSet<AppUser>();
            OnlineOrder = new HashSet<OnlineOrder>();
            Pos = new HashSet<Pos>();
            ScheduleDetail = new HashSet<ScheduleDetail>();
            UserAbility = new HashSet<UserAbility>();
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public bool Active { get; set; }
        public string Hr { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? ManagerId { get; set; }

        public AppUser Manager { get; set; }
        public UserRole Role { get; set; }
        public ICollection<AppUser> InverseManager { get; set; }
        public ICollection<OnlineOrder> OnlineOrder { get; set; }
        public ICollection<Pos> Pos { get; set; }
        public ICollection<ScheduleDetail> ScheduleDetail { get; set; }
        public ICollection<UserAbility> UserAbility { get; set; }
    }
}
