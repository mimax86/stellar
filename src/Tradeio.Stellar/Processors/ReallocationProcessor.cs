 using System;
 using System.Linq;
 using System.Runtime.InteropServices;
 using Microsoft.Extensions.Logging;
 using Tradeio.Balance;
 using Tradeio.Email;
 using Tradeio.Email.Parameters;
 using Tradeio.Stellar.Configuration;
 using Tradeio.Stellar.Data;
 using Tradeio.Stellar.Processors.Timing;

namespace Tradeio.Stellar.Processors
{
    public class ReallocationProcessor
    {
        private readonly IBalanceService _balanceService;
        private readonly IStellarRepository _stellarRepository;
        private readonly IStellarService _stellarService;
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly IEmailService _emailService;
        private readonly ITimer _timer;
        private readonly ILogger _logger;

        public ReallocationProcessor(IBalanceService balanceService, IStellarRepository stellarRepository,
            IStellarService stellarService, IStellarConfigurationService stellarConfigurationService,
            ITimerFactory timerFactory, IEmailService emailService, ILoggerFactory loggerFactory)
        {
            _balanceService = balanceService;
            _stellarRepository = stellarRepository;
            _stellarService = stellarService;
            _stellarConfigurationService = stellarConfigurationService;
            _emailService = emailService;
            _timer = timerFactory.Create(ProcessReallocation);
            _logger = loggerFactory.CreateLogger<WithdrawalProcessor>();
        }

        public void Start()
        {
            _timer.Start();
        }

        private async void ProcessReallocation()
        {
            var hotWalletBalance = await _stellarService.GetHotWalletBalanceAsync();
            var requests = await _stellarRepository.GetPendingWithdrawalRequestsAsync();
            var pendingWithdrawalsAmount = requests.Sum(request => request.Amount);

            var amount = hotWalletBalance - pendingWithdrawalsAmount;
            if (amount < _stellarConfigurationService.HotWalletThreshold)
                return;

            try
            {
                var exceedingAmount = amount - _stellarConfigurationService.HotWalletThreshold;
                await _stellarService.SubmitPaymentAsync(_stellarConfigurationService.Cold.Public, exceedingAmount);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Failed to submit rellocation transaction", e);
                _emailService.Send(new ReallocationFailedEmailParameters(
                    /*Notify that reallocation failed, potential security
                     incident because hot wallet holds a lot of funds under the risk*/));
            }
        }
    }
}