using System;
using System.Net.Http;
using Tradeio.Stellar.Deposit.Client;
using Tradeio.Stellar.Withdrawal.Client;

namespace Tradeio.Platform
{
    class Program
    {
        static void Main(string[] args)
        {
            var stellarDepositService = "http://localhost:50390";
            var stellarWithdrawalService = "http://localhost:50387";

            IDepositsClient depositClient =
                new DepositsClient(new HttpClient {BaseAddress = new Uri(stellarDepositService)});

            IWithdrawalsClient withdrawalClient =
                new WithdrawalsClient(new HttpClient { BaseAddress = new Uri(stellarWithdrawalService) });

            var newAddress = depositClient.GetTraderAddress(1);

            var newWithdrawalRequest = withdrawalClient.CreateWithdrawalRequest(1, "address", 10.23m);
        }
    }
}