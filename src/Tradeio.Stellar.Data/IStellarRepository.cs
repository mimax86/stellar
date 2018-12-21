using System.Collections.Generic;
using System.Threading.Tasks;
using Tradeio.Stellar.Data.Model;

namespace Tradeio.Stellar.Data
{
    public interface IStellarRepository
    {
        Task<TraderAddress> GetTraderAddressByTraderIdAsync(long traderId);

        Task<TraderAddress> GetTraderAddressByCustomerIdAsync(string customer);

        Task AddTraderAddressAsync(TraderAddress traderAddress);

        Task CreateTransactionAsync(TraderAddress traderAddress, decimal amount);

        Task<string> GetLastCursorAsync();

        Task AddCursorAsync(string cursor);

        Task<IReadOnlyCollection<WithdrawalRequest>> GetWithdrawalRequests();

        Task<WithdrawalRequest> AddWithdrawalRequestAsync(long traderId, string address, decimal amount);
    }
}