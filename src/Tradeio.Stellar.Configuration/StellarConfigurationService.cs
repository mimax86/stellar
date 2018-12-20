using Microsoft.Extensions.Configuration;

namespace Tradeio.Stellar.Configuration
{
    public class StellarConfigurationService : IStellarConfigurationService
    {
        private readonly StellarSettings _settings;

        public StellarConfigurationService(IConfiguration configuration)
        {
            _settings = configuration.GetSection("stellar").Get<StellarSettings>();
        }

        public string HorizonUrl => _settings.HorizonUrl;

        public WalletSettings Hot => _settings.Hot;

        public WalletSettings Cold => _settings.Cold;

        public decimal Fee => _settings.Fee;
    }
}