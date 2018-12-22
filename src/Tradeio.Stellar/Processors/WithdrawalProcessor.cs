using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;
using Tradeio.Stellar.Data.Model;
using Tradeio.Stellar.Processors.Timing;

namespace Tradeio.Stellar.Processors
{
    public class WithdrawalProcessor
    {
        private readonly IBalanceService _balanceService;
        private readonly IStellarRepository _stellarRepository;
        private readonly IStellarClient _stellarClient;
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly IEmailService _emailService;
        private readonly ITimer _timer;
        private readonly ILogger _logger;

        public WithdrawalProcessor(IBalanceService balanceService, IStellarRepository stellarRepository,
            IStellarClient stellarClient, IStellarConfigurationService stellarConfigurationService,
            ITimerFactory timerFactory, IEmailService emailService, ILoggerFactory loggerFactory)
        {
            _balanceService = balanceService;
            _stellarRepository = stellarRepository;
            _stellarClient = stellarClient;
            _stellarConfigurationService = stellarConfigurationService;
            _emailService = emailService;
            _timer = timerFactory.Create(ProcessWithdrawals);
            _logger = loggerFactory.CreateLogger<WithdrawalProcessor>();
        }

        public void Start()
        {
            _timer.Start();
        }

        private void ProcessWithdrawals()
        {
            var requests = _stellarRepository.GetPendingWithdrawalRequestsAsync().Result;
            foreach (var request in requests)
            {
                _stellarRepository.ChangeWithdrawalRequestStatus(request, WithdrwalRequestStatus.Processing);

                var balance = _balanceService.GetBalance(request.TraderId, Asset.Lumen);
                if (balance < request.Amount)
                {
                    _emailService.Send(
                        new EmailParameters( /*Notify that somehow balance is not enough to process withdrawal*/));
                    continue;
                }

                var hotWalletBalance = _stellarClient.GetHotWalletBalance();
                if (hotWalletBalance < request.Amount)
                {
                    _emailService.Send(
                        new EmailParameters( /*Notify that hot wallet refill is needed*/));
                    continue;
                }

                try
                {
                    _stellarClient.SubmitPayment(request.Address, request.Amount);
                    _balanceService.Withdraw(request.TraderId, request.Amount, Asset.Lumen);
                    _stellarRepository.ChangeWithdrawalRequestStatus(request, WithdrwalRequestStatus.Completed);
                }
                catch (Exception e)
                {
                    _logger.LogCritical("Failure in processing withdrawal", e);
                    _stellarRepository.ChangeWithdrawalRequestStatus(request, WithdrwalRequestStatus.Error);
                }
            }
        }
    }
}