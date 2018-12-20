using System.Threading.Tasks;
using Tradeio.Stellar.Data.Model;

namespace Tradeio.Stellar.Data
{
    public interface IStellarRepository
    {
        Task<TraderAddress> GetTraderAddressByTraderIdAsync(long traderId);

        Task<TraderAddress> GetTraderAddressByCustomerIdAsync(string customer);

        Task AddTraderAddressAsync(TraderAddress traderAddress);

        Task CreateTransactionAsync(TraderAddress traderAddress, string paymentAmount);

        Task<string> GetLastCursorAsync();

        Task AddCursorAsync(string cursor);
    }
}