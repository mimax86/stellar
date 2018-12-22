using System;
using Microsoft.Extensions.Logging;
using Tradeio.Stellar.Configuration;

namespace Tradeio.Stellar.Processors.Timing
{
    public class TimerFactory : ITimerFactory
    {
        private readonly IStellarConfigurationService _stellarConfigurationService;
        private readonly ILoggerFactory _loggerFactory;

        public TimerFactory(IStellarConfigurationService stellarConfigurationService,
            ILoggerFactory loggerFactory)
        {
            _stellarConfigurationService = stellarConfigurationService;
            _loggerFactory = loggerFactory;
        }

        public ITimer Create(Action handler)
        {
            return new Timer(_stellarConfigurationService, _loggerFactory, handler);
        }
    }
}