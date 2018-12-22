using System;
using stellar_dotnet_sdk.responses.operations;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;

namespace Tradeio.Stellar.Processors
{
    public class DepositProcessor : IDisposable
    {
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly IStellarClient _stellarClient;
        private readonly IStellarRepository _stellarRepository;
        private readonly IBalanceService _balanceService;
        private readonly IEmailService _emailService;

        public DepositProcessor(
            IStellarConfigurationService stellarConfigurationService,
            IStellarClient stellarClient,
            IStellarRepository stellarRepository,
            IBalanceService balanceService,
            IEmailService emailService)
        {
            _stellarConfigurationService = stellarConfigurationService;
            _stellarClient = stellarClient;
            _stellarRepository = stellarRepository;
            _balanceService = balanceService;
            _emailService = emailService;
        }

        public void Start()
        {
            var lastCursor = _stellarRepository.GetLastCursorAsync().Result;

            _stellarClient.ListenHotWallet(lastCursor, (sender, response) =>
            {
                if (!(response is PaymentOperationResponse payment))
                    return;

                var transaction = _stellarClient.GetTransaction(payment.TransactionHash).Result;

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
                    _emailService.Send(new EmailParameters( /*notify got unregistered trader deposit*/));
                }
            });
        }

        public void Dispose()
        {
            _stellarClient?.Dispose();
        }
    }
}