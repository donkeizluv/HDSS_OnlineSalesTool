using OnlineSalesTool.EFModel;
using SyncService.SyncLogic.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncService.SyncLogic.Impl.Test
{
    public class FakeIndusAdapter : IIndusAdapter
    {
        public async Task ReflectChanges(IEnumerable<OnlineOrder> orders)
        {
            //Set INDUS status, isDirty here to progress cases
            foreach (var order in orders)
            {
                order.Address += " ....indus reflect...";
            }
            await Task.Delay(3);
        }
    }
}
