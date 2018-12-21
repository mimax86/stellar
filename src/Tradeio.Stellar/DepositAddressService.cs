using stellar_dotnet_sdk;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Interfaces;

namespace Tradeio.Stellar
{
    public class DepositAddressService : IDepositAddressService
    {
        private readonly IStellarConfigurationService _settingsService;

        public DepositAddressService(IStellarConfigurationService settingsService)
        {
            _settingsService = settingsService;
        }

        public string GetNewAddress()
        {
            Server server = new Server(_settingsService.Server);
            server.Accounts.Execute<>()


            var stream = server.Payments.ForAccount().Stream((sender, response) =>
            {

            });
            stream.Connect()


            var keyPair = KeyPair.();
            return keyPair.Address;
        }

        public string GetColdWalletAddress()
        {
            throw new System.NotImplementedException();
        }
    }
}