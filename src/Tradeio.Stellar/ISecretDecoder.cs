namespace Tradeio.Stellar
{
    public interface ISecretDecoder
    {
        string Decode(string value);
    }
}