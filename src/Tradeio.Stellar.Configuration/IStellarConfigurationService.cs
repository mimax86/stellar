namespace Tradeio.Stellar.Configuration
{
    public interface IStellarConfigurationService
    {
        string Server { get; }

        string Seed { get; }

        decimal Fee { get; }
    }
}