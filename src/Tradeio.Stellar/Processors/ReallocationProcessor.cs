namespace Tradeio.Stellar.Processors
{
    public class ReallocationProcessor
    {
        public void Start()
        {
            /*Move funds from hot wallet to cold wallet by timer
              Check current hot wallet amount
              If it exceeds threshold then create payment transaction with exceeded value being moved to cold wallet
              When assesing exceeded value need to deduct value of pending withdrawal requests*/
        }
    }
}