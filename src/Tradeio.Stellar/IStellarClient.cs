using System;
using System.Threading.Tasks;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace Tradeio.Stellar
{
    public interface IStellarClient : IDisposable
    {
        EventHandler<OperationResponse> ListenAccountOperations(string account, string cursor, EventHandler<OperationResponse> handler);

        Task<TransactionResponse> GetTransaction(string transactionId);
    }
}