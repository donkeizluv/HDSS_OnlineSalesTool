using Microsoft.EntityFrameworkCore;
using OnlineSalesCore.Const;

namespace OnlineSalesCore.EFModel
{
    public partial class OnlineSalesContext
    {
        //preserve
        //public OnlineSalesContext() : base(AppState.DbContextOption)
        //{

        //}
        public OnlineSalesContext(DbContextOptions<OnlineSalesContext> options) : base(options)
        {

        }
        public OnlineSalesContext(string conStr) : base(new DbContextOptionsBuilder().UseSqlServer(conStr).Options)
        {

        }
        //preserve
    }
}
