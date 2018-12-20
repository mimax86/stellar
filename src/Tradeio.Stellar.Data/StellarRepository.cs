using System.Threading.Tasks;
using Tradeio.Stellar.Data.Model;

namespace Tradeio.Stellar.Data
{
    public class StellarRepository : IStellarRepository
    {
        public Task<TraderAddress> GetTraderAddressByTraderIdAsync(long traderId)
        {
            throw new System.NotImplementedException();
        }

        public Task<TraderAddress> GetTraderAddressByCustomerIdAsync(string customer)
        {
            throw new System.NotImplementedException();
        }

        public Task AddTraderAddressAsync(TraderAddress traderAddress)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateTransactionAsync(TraderAddress traderAddress, string paymentAmount)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetLastCursorAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task AddCursorAsync(string cursor)
        {
            throw new System.NotImplementedException();
        }
    }
}