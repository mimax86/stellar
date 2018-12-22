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
    public class StellarService : IStellarService
    {
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly ISecretDecoder _secretDecoder;
        private readonly Server _server;
        private readonly KeyPair _hotWallet;
        private readonly KeyPair _coldWallet;

        public StellarService(
            IStellarConfigurationService stellarConfigurationService,
            ISecretDecoder secretDecoder)
        {
            _stellarConfigurationService = stellarConfigurationService;
            _secretDecoder = secretDecoder;
            _server = new Server(stellarConfigurationService.HorizonUrl);
            _hotWallet = KeyPair.FromSecretSeed(stellarConfigurationService.Hot.Secret);
            _coldWallet = KeyPair.FromAccountId(stellarConfigurationService.Cold.Public);
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

        public async Task<TransactionResponse> GetTransactionAsync(string transactionId)
        {
            return await _server.Transactions.Transaction(transactionId);
        }

        public async Task<decimal> GetHotWalletBalanceAsync()
        {
            var account = await _server.Accounts.Account(_hotWallet);
            var value = account.Balances
                .FirstOrDefault(balance => balance.AssetType == new AssetTypeNative().GetType())?.BalanceString;
            return GetValue(value);
        }

        public async Task<string> SubmitHotWalletWithdrawalAsync(string destinationAddress, decimal amount)
        {
            var hotWalletAccout = await _server.Accounts.Account(_hotWallet);
            var account = new Account(_hotWallet, hotWalletAccout.SequenceNumber);
            var transaction = new Transaction.Builder(account)
                .AddOperation(new PaymentOperation.Builder(KeyPair.FromAccountId(destinationAddress),
                    new AssetTypeNative(),
                    GetString(amount)).Build())
                .Build();
            transaction.Sign(_hotWallet);
            var result = await _server.SubmitTransaction(transaction);
            if (!result.IsSuccess())
                throw new StellarServiceException(
                    $"Failed to submit transaction to blockchain: error {result.SubmitTransactionResponseExtras.ExtrasResultCodes.TransactionResultCode}");
            return result.Hash;
        }

        public async Task<string> SubmitColdWalletWithdrawalAsync(string destinationAddress, decimal amount, string[] secrets)
        {
            var coldWalletAccount = await _server.Accounts.Account(_coldWallet);
            var account = new Account(_coldWallet, coldWalletAccount.SequenceNumber);
            var transaction = new Transaction.Builder(account)
                .AddOperation(new PaymentOperation.Builder(KeyPair.FromAccountId(destinationAddress),
                    new AssetTypeNative(),
                    GetString(amount)).Build())
                .Build();
            foreach (var signature in secrets)
            {
                var secret = _secretDecoder.Decode(signature);
                transaction.Sign(KeyPair.FromSecretSeed(secret));
            }

            var result = await _server.SubmitTransaction(transaction);
            if (!result.IsSuccess())
                throw new StellarServiceException(
                    $"Failed to submit transaction to blockchain: error {result.SubmitTransactionResponseExtras.ExtrasResultCodes.TransactionResultCode}");
            return result.Hash;
        }

        private decimal GetValue(string value)
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
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