namespace Tradeio.Stellar.Configuration
{
    public interface IStellarConfigurationService
    {
        string HorizonUrl { get; }

        WalletSettings Hot { get; }

        WalletSettings Cold { get; }

        decimal Fee { get; }

        int PollingInterval { get; }
    }
}