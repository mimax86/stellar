using System;
using System.Threading.Tasks;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;
using Tradeio.Stellar.Data.Model;

namespace Tradeio.Stellar.Deposit.Controllers
{
    public class DepositsControllerInternal : IDepositsController
    {
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly IStellarRepository _stellarRepository;

        public DepositsControllerInternal(IStellarConfigurationService stellarConfigurationService,
            IStellarRepository stellarRepository)
        {
            _stellarConfigurationService = stellarConfigurationService;
            _stellarRepository = stellarRepository;
        }

        public async Task<DepositAddressModel> GetTraderAddressAsync(long traderId)
        {
            var traderAddress = await _stellarRepository.GetTraderAddressByTraderIdAsync(traderId);
            if (traderAddress == null)
            {
                traderAddress = new TraderAddress
                {
                    TraderId = traderId,
                    CustomerId = Guid.NewGuid().ToString("N")
                };
                await _stellarRepository.AddTraderAddressAsync(traderAddress);
            }

            return new DepositAddressModel
            {
                TraderId = traderId,
                CustomerId = traderAddress.CustomerId,
                Address = _stellarConfigurationService.Hot.Public
            };
        }
    }
}