using System.Threading.Tasks;
using Tradeio.Stellar.Interfaces;

namespace Tradeio.Stellar.Deposit.Controllers
{
    public class DepositsControllerInternal : IDepositsController
    {
        private readonly IDepositAddressService _depositAddressService;

        public DepositsControllerInternal(IDepositAddressService depositAddressService)
        {
            _depositAddressService = depositAddressService;
        }

        public async Task<DepositAddressModel> GetTraderAddressAsync(long traderId)
        {
            var address = _depositAddressService.GetNewAddress();
            return new DepositAddressModel
            {
                TraderId = traderId,
                Address = address
            };
        }
    }
}