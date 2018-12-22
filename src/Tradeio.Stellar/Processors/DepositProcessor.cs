using System;
using stellar_dotnet_sdk.responses.operations;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Email.Parameters;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;

namespace Tradeio.Stellar.Processors
{
    public class DepositProcessor : IDisposable
    {
        private readonly IStellarService _stellarService;
        private readonly IStellarRepository _stellarRepository;
        private readonly IBalanceService _balanceService;
        private readonly IEmailService _emailService;

        public DepositProcessor(
            IStellarService stellarService,
            IStellarRepository stellarRepository,
            IBalanceService balanceService,
            IEmailService emailService)
        {
            _stellarService = stellarService;
            _stellarRepository = stellarRepository;
            _balanceService = balanceService;
            _emailService = emailService;
        }

        public void Start()
        {
            var lastCursor = _stellarRepository.GetLastCursorAsync().Result;

            _stellarService.ListenHotWallet(lastCursor, (sender, response) =>
            {
                if (!(response is PaymentOperationResponse payment))
                    return;

                var transaction = _stellarService.GetTransactionAsync(payment.TransactionHash).Result;

                var custometId = transaction.MemoStr;

                var traderAddress = _stellarRepository.GetTraderAddressByCustomerIdAsync(custometId).Result;
                if (traderAddress != null)
                {
                    var amount = Convert.ToDecimal(payment.Amount);
                    _stellarRepository.CreateTransactionAsync(traderAddress, amount);
                    _stellarRepository.AddCursorAsync(response.PagingToken);
                    _balanceService.Deposit(traderAddress.TraderId, Convert.ToDecimal(amount),
                        payment.AssetCode);
                }
                else
                {
                    _emailService.Send(
                        new UnregisteredTraderDepositEmailParameters(
                            /*Notify that received deposit from unregistered trader*/));
                }
            });
        }

        public void Dispose()
        {
            _stellarService?.Dispose();
        }
    }
}