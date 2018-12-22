using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Email.Parameters;
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
        private readonly IStellarService _stellarService;
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly IEmailService _emailService;
        private readonly ITimer _timer;
        private readonly ILogger _logger;

        public WithdrawalProcessor(IBalanceService balanceService, IStellarRepository stellarRepository,
            IStellarService stellarService, IStellarConfigurationService stellarConfigurationService,
            ITimerFactory timerFactory, IEmailService emailService, ILoggerFactory loggerFactory)
        {
            _balanceService = balanceService;
            _stellarRepository = stellarRepository;
            _stellarService = stellarService;
            _stellarConfigurationService = stellarConfigurationService;
            _emailService = emailService;
            _timer = timerFactory.Create(ProcessWithdrawals);
            _logger = loggerFactory.CreateLogger<WithdrawalProcessor>();
        }

        public void Start()
        {
            _timer.Start();
        }

        private async void ProcessWithdrawals()
        {
            var pendingWithdrawalRequests = await _stellarRepository.GetPendingWithdrawalRequestsAsync();
            foreach (var request in pendingWithdrawalRequests)
            {
                await _stellarRepository.ChangeWithdrawalRequestStatusAsync(request, WithdrwalRequestStatus.Processing);

                var balance = _balanceService.GetBalance(request.TraderId, Asset.Lumen);
                if (balance < request.Amount)
                {
                    _emailService.Send(
                        new InsufficientTraderFundsForWithrawalEmailParameters( /*Notify that trader balance is not enough to process withdrawal.
                                               This might indicate the flaw in logic allowing trader to initiate withdrawal
                                               not having enough funds*/));
                    continue;
                }

                var hotWalletBalance = await _stellarService.GetHotWalletBalanceAsync();
                if (hotWalletBalance < request.Amount)
                {
                    _emailService.Send(
                        new InsufficienHotWalletFundsForWithdrawalEmailParameters( /*Notify that hot wallet refill is needed*/));
                    continue;
                }

                try
                {
                    await _stellarService.SubmitHotWalletWithdrawalAsync(request.Address, request.Amount);
                    _balanceService.Withdraw(request.TraderId, request.Amount, Asset.Lumen);
                    await _stellarRepository.ChangeWithdrawalRequestStatusAsync(request, WithdrwalRequestStatus.Completed);
                }
                catch (Exception e)
                {
                    _logger.LogCritical("Failure in processing withdrawal", e);
                    await _stellarRepository.ChangeWithdrawalRequestStatusAsync(request, WithdrwalRequestStatus.Error);
                }
            }
        }
    }
}