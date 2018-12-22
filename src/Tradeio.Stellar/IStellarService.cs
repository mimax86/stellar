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

        /// <summary>
        /// Creates, signs and submits transaction with payment operation from hot wallet
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<string> SubmitHotWalletWithdrawalAsync(string destinationAddress, decimal amount);

        /// <summary>
        /// Creates, signs and submits transaction with payment operation from cold multisig wallet
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <param name="secrets">Encrypted secret seed to be used to sign transaction</param>
        /// <returns>Transaction hash</returns>
        Task<string> SubmitColdWalletWithdrawalAsync(string destinationAddress, decimal amount, string[] secrets);
    }
}