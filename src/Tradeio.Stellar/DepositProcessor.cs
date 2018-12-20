using System;
using System.Text;
using Tradeio.Stellar.Data;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses.operations;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Stellar.Configuration;

namespace Tradeio.Stellar
{
    public class DepositProcessor : IDisposable
    {
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly IStellarRepository _stellarRepository;
        private readonly IBalanceService _balanceService;
        private readonly IEmailService _emailService;
        private readonly Server _server;

        public DepositProcessor(
            IStellarConfigurationService stellarConfigurationService,
            IStellarRepository stellarRepository,
            IBalanceService balanceService,
            IEmailService emailService)
        {
            _stellarConfigurationService = stellarConfigurationService;
            _stellarRepository = stellarRepository;
            _balanceService = balanceService;
            _emailService = emailService;
            _server = new Server(stellarConfigurationService.HorizonUrl);
        }

        public void Start()
        {
            var builder =
                _server.Operations.ForAccount(
                    KeyPair.FromPublicKey(Encoding.Unicode.GetBytes(_stellarConfigurationService.Hot.Public)));

            var lastCursor = _stellarRepository.GetLastCursorAsync().Result;
            if (!string.IsNullOrEmpty(lastCursor))
                builder.Cursor(lastCursor);

            builder.Stream(HandlePayment).Connect();
        }

        private void HandlePayment(object sender, OperationResponse e)
        {
            if (!(e is PaymentOperationResponse payment))
                return;
            var transaction = _server.Transactions.Transaction(payment.TransactionHash).Result;
            var custometId = transaction.MemoStr;

            var traderAddress = _stellarRepository.GetTraderAddressByCustomerIdAsync(custometId).Result;
            if (traderAddress != null)
            {
                _stellarRepository.CreateTransactionAsync(traderAddress, payment.Amount);
                _stellarRepository.AddCursorAsync(e.PagingToken);

                _balanceService.Change(traderAddress.TraderId, Convert.ToDecimal(payment.Amount), payment.AssetCode);
                _emailService.Send(new EmailParameters( /*some email parameters*/));
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}