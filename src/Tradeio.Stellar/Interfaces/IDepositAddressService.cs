namespace Tradeio.Stellar.Interfaces
{
    public interface IDepositAddressService
    {
        string GetNewAddress();

        string GetColdWalletAddress();
    }
}