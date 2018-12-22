using System;
using System.Threading.Tasks;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace Tradeio.Stellar
{
    public interface IStellarService : IDisposable
    {
        EventHandler<OperationResponse> ListenHotWallet(string cursor,
            EventHandler<OperationResponse> handler);

        Task<TransactionResponse> GetTransactionAsync(string transactionId);

        Task<decimal> GetHotWalletBalanceAsync();

        Task SubmitPaymentAsync(string destinationAddress, decimal amount);
    }
}