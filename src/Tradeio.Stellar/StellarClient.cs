using System;
using System.Text;
using System.Threading.Tasks;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace Tradeio.Stellar
{
    public class StellarClient : IStellarClient
    {
        private readonly Server _server;

        public StellarClient(string url)
        {
            _server = new Server(url);
        }

        public EventHandler<OperationResponse> ListenAccountOperations(string account, string cursor,
            EventHandler<OperationResponse> handler)
        {
            var builder =
                _server.Operations.ForAccount(
                    KeyPair.FromPublicKey(Encoding.Unicode.GetBytes(account)));
            if (!string.IsNullOrEmpty(cursor))
                builder.Cursor(cursor);
            builder.Stream(handler).Connect();
            return handler;
        }

        public async Task<TransactionResponse> GetTransaction(string transactionId)
        {
            return await _server.Transactions.Transaction(transactionId);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}