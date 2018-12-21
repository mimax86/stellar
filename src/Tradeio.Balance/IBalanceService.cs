namespace Tradeio.Balance
{
    public interface IBalanceService
    {
        void Deposit(long traderId, decimal amount, string asset);

        void Withdraw(long traderId, decimal amount, string asset);

        decimal GetBalance(long traderId, string asset);
    }
}