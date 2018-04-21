using OnlineSalesTool.AppEnum;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Service;
using SyncService.SyncLogic.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncService.SyncLogic.Impl.Test
{
    public class FakeDealerAPI : IDealerAPI
    {
        private Random _rnd = new Random();

        public Task<List<OnlineOrder>> FetchNewCase()
        {
            var orders = new List<OnlineOrder>();
            for (int i = 0; i < _rnd.Next(5, 10); i++)
            {
                orders.Add(new OnlineOrder()
                {
                    TrackingNumber = DevUlt.RandomString(10, _rnd),
                    Name = DevUlt.RandomString(5, _rnd),
                    NatId = DevUlt.RandomString(5, _rnd),
                    Phone = DevUlt.RandomString(5, _rnd),
                    Address = DevUlt.RandomString(5, _rnd),
                    PosCode = DevUlt.RandomString(5, _rnd),
                    Product = DevUlt.RandomString(5, _rnd),
                    Amount = 123,
                    Paid = 50,
                    LoanAmount = 73,
                    Term = 24,
                    Received = DateTime.Now,
                    StageId = (int)Stage.NotAssigned, //Newly received cases start at stage: 0
                    IsDirty = false //Dirty mean case status need to be updated to dealer
                });
            }
            return Task.FromResult(orders);
        }

        public Task<string> GetOrderNumber(string trackingNumber)
        {
            return Task.FromResult(DevUlt.RandomString(5, _rnd));
        }

        public Task RequestOrderNumber(OnlineOrder order)
        {
            return Task.FromResult(0);
        }

        public Task RequestOrderNumbers(List<OnlineOrder> orders)
        {
            return Task.FromResult(0);
        }

        public Task UpdateStatus(List<OnlineOrder> orders)
        {
            return Task.FromResult(0);
        }

        public Task UpdateStatus(OnlineOrder order)
        {
            return Task.FromResult(0);
        }
    }
}
