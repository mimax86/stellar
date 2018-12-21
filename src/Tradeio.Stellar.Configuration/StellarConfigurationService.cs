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

        public string Server => _settings.Server;

        public string Seed => _settings.Seed;

        public decimal Fee => _settings.Fee;
    }
}