using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.xdr;
using Tradeio.Stellar.Configuration;

namespace Tradeio.Stellar.Tests
{
    public class StellarServiceTests
    {
        private IStellarService _stellarService;
        private IStellarConfigurationService _stellarConfigurationService;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            _stellarConfigurationService = new StellarConfigurationService(configuration);
            _stellarService = new StellarService(_stellarConfigurationService);
            Network.UseTestNetwork();
        }

        [Test]
        public async Task StellarService_Returns_Transaction_By_Id()
        {
            var transaction = await _stellarService.GetTransactionAsync(
                "9107ec84c295dd2394ea4195e5f980a3760e47f852c47a9ef74822421f43db15");
            transaction.MemoStr.Should().Be("123");
        }

        [Test]
        public async Task StellarService_Returns_HotWallet_Balance()
        {
            var value = await _stellarService.GetHotWalletBalanceAsync();
            value.Should().BePositive();
        }

        [Test]
        public async Task StellarService_Submits_Withdrwal_From_Hot_To_Cold_Storage()
        {
            var transactionHash =
                await _stellarService.SubmitHotWalletWithdrawalAsync(_stellarConfigurationService.Cold.Public, 1m);
            //Without delay might get NotFoundException requesting submitted transacion
            Thread.Sleep(1000);
            var transaction = await _stellarService.GetTransactionAsync(transactionHash);
            transaction.OperationCount.Should().Be(1);
        }

        [Test]
        public async Task StellarService_Submits_Withdrawal_From_Cold_Storage()
        {
            var transactionHash =
                await _stellarService.SubmitColdWalletWithdrawalAsync(_stellarConfigurationService.Hot.Public, 1m,
                    new[]
                    {
                        "SAPH3O6B5CMTPULJRGJOPD2CGWLFIC2T742RIMCMA2YR32BMZLVSLUBR",
                        "SBQS373RLZ72BBNJLAOAD2SXJYMNHR3XB6BBJLOP3WLKVYOWAT5SWBKG"
                    });
            //Without delay might get NotFoundException requesting submitted transacion
            Thread.Sleep(1000);
            var transaction = await _stellarService.GetTransactionAsync(transactionHash);
            transaction.OperationCount.Should().Be(1);
        }

        [Test]
        public void StellarService_Throws_Exceptions_If_Not_Enough_Signatures_For_Cold_Wallet_Withdrawal()
        {
            Action action = () =>
            {
                _stellarService.SubmitColdWalletWithdrawalAsync(_stellarConfigurationService.Hot.Public, 1m,
                    new[]
                    {
                        "SAPH3O6B5CMTPULJRGJOPD2CGWLFIC2T742RIMCMA2YR32BMZLVSLUBR"
                    }).Wait();
            };
            action.Should().Throw<StellarServiceException>().WithMessage("*tx_bad_auth");
        }

        [Test]
        public void StellarService_Throws_Exceptions_If_Not_Invalid_Signatures_For_Cold_Wallet_Withdrawal()
        {
            Action action = () =>
            {
                _stellarService.SubmitColdWalletWithdrawalAsync(_stellarConfigurationService.Hot.Public, 1m,
                    new[]
                    {
                        "SB5BAYX4XO3FW3KCDRSI3TCLHDPNFOYKXEFZ4N7X6AFUWGZB4I6X4MU3",
                        "SBEZBIECWMMPP3DAUI4RZ4UTWCWUEYZIUQ4JPC44PYV62GK6BB5GTSUW"
                    }).Wait();
            };
            action.Should().Throw<StellarServiceException>().WithMessage("*tx_bad_auth");
        }
    }
}