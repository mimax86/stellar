using System;
using System.Threading.Tasks;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace Tradeio.Stellar
{
    public interface IStellarClient : IDisposable
    {
        EventHandler<OperationResponse> ListenHotWallet(string cursor,
            EventHandler<OperationResponse> handler);

        Task<TransactionResponse> GetTransaction(string transactionId);

        decimal GetHotWalletBalance();

        void SubmitPayment(string requestAddress, decimal requestAmount);
    }
}