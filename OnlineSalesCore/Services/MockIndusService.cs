using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public class MockIndusService : IIndusService
    {
        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public async Task<IndusContractDTO> GetContract(string contractNumber)
        {
            return new IndusContractDTO() {
                ContractNumber = contractNumber,
                FullName = "ABC",
                NatId = "29348934",
                Product = "mock",
                Amount = 1111,
                Paid = 123,
                LoanAmount = 123,
                Term = 12
            };
            // throw new NotImplementedException();
        }
    }
}
