using System.Threading.Tasks;
using Tradeio.Stellar.Data;

namespace Tradeio.Stellar.Withdrawal.Controllers
{
    public class WithdrawalsControllerInternal : IWithdrawalsController
    {
        private readonly IStellarRepository _stellarRepository;

        public WithdrawalsControllerInternal(IStellarRepository stellarRepository)
        {
            _stellarRepository = stellarRepository;
        }

        public async Task<WithdrawalRequestModel> CreateWithdrawalRequestAsync(long traderId, string address,
            decimal amount)
        {
            var newRequest = await _stellarRepository.AddWithdrawalRequestAsync(traderId, address, amount);
            return new WithdrawalRequestModel
            {
                Address = newRequest.Address,
                Amount = newRequest.Amount,
                TraderId = newRequest.TraderId,
            };
        }
    }
}