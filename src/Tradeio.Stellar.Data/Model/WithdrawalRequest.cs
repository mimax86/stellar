namespace Tradeio.Stellar.Data.Model
{
    public class WithdrawalRequest
    {
        public long Id { get; set; }

        public long TraderId { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }

        public WithdrwalRequestStatus Status { get; set; }
    }
}