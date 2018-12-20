namespace Tradeio.Balance
{
    public interface IBalanceService
    {
        void Change(long traderId, decimal Amount, string Asset);
    }
}