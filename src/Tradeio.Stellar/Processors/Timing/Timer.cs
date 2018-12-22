using System;
using Microsoft.Extensions.Logging;
using Tradeio.Stellar.Configuration;
using InternalTimer = System.Threading.Timer;

namespace Tradeio.Stellar.Processors.Timing
{
    public class Timer : ITimer
    {
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly Action _handler;
        private readonly ILogger _logger;
        private InternalTimer _timer;

        public Timer(IStellarConfigurationService stellarConfigurationService, ILoggerFactory loggerFactory,
            Action handler)
        {
            _stellarConfigurationService = stellarConfigurationService;
            _handler = handler;
            _logger = loggerFactory.CreateLogger<Timer>();
        }

        public void Start()
        {
            _timer = new InternalTimer(state =>
                {
                    try
                    {
                        _handler();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Failed to proceess time job", e);
                    }
                }, null, _stellarConfigurationService.PollingInterval,
                _stellarConfigurationService.PollingInterval);
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}