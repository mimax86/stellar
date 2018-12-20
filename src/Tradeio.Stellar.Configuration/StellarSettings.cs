namespace Tradeio.Stellar.Configuration
{
    public class StellarSettings
    {
        public string HorizonUrl { get; set; }

        public WalletSettings Hot { get; set; }

        public WalletSettings Cold { get; set; }

        public decimal Fee { get; set; }

        public int PollingInterval { get; set; }
    }
}