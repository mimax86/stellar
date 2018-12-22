using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;
using Tradeio.Stellar.Configuration;

namespace Tradeio.Stellar
{
    public class StellarClient : IStellarClient
    {
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly Server _server;
        private readonly KeyPair _hotWallet;

        public StellarClient(IStellarConfigurationService stellarConfigurationService)
        {
            _stellarConfigurationService = stellarConfigurationService;
            _server = new Server(stellarConfigurationService.HorizonUrl);
            _hotWallet = KeyPair.FromPublicKey(Encoding.Unicode.GetBytes(stellarConfigurationService.Hot.Public));
        }

        public EventHandler<OperationResponse> ListenHotWallet(string cursor,
            EventHandler<OperationResponse> handler)
        {
            var builder =
                _server.Operations.ForAccount(_hotWallet);
            if (!string.IsNullOrEmpty(cursor))
                builder.Cursor(cursor);
            builder.Stream(handler).Connect();
            return handler;
        }

        public async Task<TransactionResponse> GetTransaction(string transactionId)
        {
            return await _server.Transactions.Transaction(transactionId);
        }

        public decimal GetHotWalletBalance()
        {
            var value = _server.Accounts.Account(_hotWallet).Result.Balances
                .FirstOrDefault(balance => balance.AssetCode == Asset.Lumen)?.BalanceString;
            return GetValue(value);
        }

        public void SubmitPayment(string requestAddress, decimal requestAmount)
        {
            var sequenceNumber = _server.Accounts.Account(_hotWallet).Result.IncrementedSequenceNumber;
            var account = new Account(_hotWallet, sequenceNumber);
            var transaction = new Transaction.Builder(account)
                .AddOperation(new PaymentOperation.Builder(KeyPair.FromAccountId(requestAddress), new AssetTypeNative(),
                    GetString(requestAmount)).Build())
                .Build();
            transaction.Sign(_hotWallet);
            var result = _server.SubmitTransaction(transaction).Result;
            if (!result.IsSuccess())
                throw new StellarClientException("Failed to submit transaction to blockchain");
        }

        private decimal GetValue(string value)
        {
            return Convert.ToDecimal(value);
        }

        private string GetString(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}