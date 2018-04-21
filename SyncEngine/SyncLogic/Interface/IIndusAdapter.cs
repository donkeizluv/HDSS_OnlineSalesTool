using OnlineSalesTool.EFModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncService.SyncLogic.Interface
{
    /// <summary>
    /// Reflect any changes of INDUS to order
    /// </summary>
    public interface IIndusAdapter
    {
        Task ReflectChanges(IEnumerable<OnlineOrder> orders);
    }
}
