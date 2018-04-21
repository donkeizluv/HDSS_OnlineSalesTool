using OnlineSalesTool.EFModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncService.SyncLogic.Interface
{
    /// <summary>
    /// API sets for dealers
    /// </summary>
    public interface IDealerAPI
    {
        /// <summary>
        /// Poll new orders from dealer
        /// </summary>
        /// <returns></returns>
        Task<List<OnlineOrder>> FetchNewCase(); //Fetch new cases
        //Since we dont know how they provide the API interface, lets assume they let us call with an array of order


        /// <summary>
        /// Update/reflect changes of orders to dealer
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        Task UpdateStatus(List<OnlineOrder> orders);
        Task UpdateStatus(OnlineOrder order);


        /// <summary>
        /// Request online order number for collection of orders
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        Task RequestOrderNumbers(List<OnlineOrder> orders);
        Task RequestOrderNumber(OnlineOrder order);


        /// <summary>
        /// Check to see if dealer have generated this order online bill number
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        Task<string> GetOrderNumber(string trackingNumber);
    }
}
