using System.Collections.ObjectModel;
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

        Task<ReadOnlyCollection<WithdrawalRequest>> GetPendingWithdrawalRequestsAsync();

        Task<WithdrawalRequest> AddWithdrawalRequestAsync(long traderId, string address, decimal amount);

        Task ChangeWithdrawalRequestStatusAsync(WithdrawalRequest request, WithdrwalRequestStatus status);
    }
}