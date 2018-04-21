using OnlineSalesTool.AppEnum;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    public class OrderRepository
    {
        private readonly OnlineSalesContext _dbContext;
        public OrderRepository(OnlineSalesContext context)
        {
            _dbContext = context;
        }
        public IQueryable<OnlineOrder> GetNotAssigned()
        {
            return _dbContext.OnlineOrder.Where(o => o.StageId == (int)Stage.NotAssigned);
        }
    }
}
