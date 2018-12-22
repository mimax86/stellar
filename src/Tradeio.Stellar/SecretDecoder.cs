namespace Tradeio.Stellar
{
    public class SecretDecoder : ISecretDecoder
    {
        public string Decode(string value)
        {
            //Should have custom algoritm to decode encrypted secrets
            return value;
        }
    }
}