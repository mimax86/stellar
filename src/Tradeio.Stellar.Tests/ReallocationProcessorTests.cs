using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using stellar_dotnet_sdk;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Email.Parameters;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;
using Tradeio.Stellar.Data.Enum;
using Tradeio.Stellar.Data.Model;
using Tradeio.Stellar.Processors;
using Tradeio.Stellar.Processors.Timing;

namespace Tradeio.Stellar.Tests
{
    public class ReallocationProcessorTests
    {
        private IStellarConfigurationService _stellarConfigurationService;
        private ReallocationProcessor _reallocationProcessor;
        private Mock<IStellarService> _stellarServiceMock;
        private Mock<IStellarRepository> _stellarRepositoryMock;
        private Mock<IBalanceService> _balanceServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Action _timerHandler;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            _stellarConfigurationService = new StellarConfigurationService(configuration);
            _stellarRepositoryMock = new Mock<IStellarRepository>();
            _balanceServiceMock = new Mock<IBalanceService>();
            _emailServiceMock = new Mock<IEmailService>();
            _stellarRepositoryMock = new Mock<IStellarRepository>();
            _stellarServiceMock = new Mock<IStellarService>();
            var timerFactoryMock = new Mock<ITimerFactory>();
            timerFactoryMock.Setup(factory => factory.Create(It.IsAny<Action>()))
                .Returns(new Mock<ITimer>().Object)
                .Callback<Action>(handler => _timerHandler = handler);

            _reallocationProcessor = new ReallocationProcessor(
                _balanceServiceMock.Object,
                _stellarRepositoryMock.Object,
                _stellarServiceMock.Object,
                _stellarConfigurationService,
                timerFactoryMock.Object,
                _emailServiceMock.Object,
                new NullLoggerFactory());
        }

        [Test]
        public void ReallocationProcessor_Does_Not_Submit_Transaction_If_Threshold_Is_Not_Violated()
        {
            _stellarServiceMock.Setup(service => service.GetHotWalletBalanceAsync())
                .Returns(Task.FromResult(5000m));
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(Enumerable.Empty<WithdrawalRequest>().ToList().AsReadOnly()));
            _timerHandler.Invoke();
            _stellarServiceMock.Verify(service => service.SubmitHotWalletWithdrawalAsync(It.IsAny<string>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Test]
        public void ReallocationProcessor_Submits_Transaction_If_Threshold_Is_Violated()
        {
            var currentHotWalletBalance = 20000m;
            _stellarServiceMock.Setup(service => service.GetHotWalletBalanceAsync())
                .Returns(Task.FromResult(currentHotWalletBalance));
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(Enumerable.Empty<WithdrawalRequest>().ToList().AsReadOnly()));
            _timerHandler.Invoke();
            _stellarServiceMock.Verify(
                service => service.SubmitHotWalletWithdrawalAsync(
                    It.Is<string>(value => value == _stellarConfigurationService.Cold.Public),
                    It.Is<decimal>(value =>
                        value == currentHotWalletBalance - _stellarConfigurationService.HotWalletThreshold)));
        }

        [Test]
        public void ReallocationProcessor_Submits_Transaction_If_Threshold_Is_Violated_Considering_Pending_Withdrawals()
        {
            var currentHotWalletBalance = 20000m;
            var pendingWithdrawals = GetWithdrawalRequests();
            _stellarServiceMock.Setup(service => service.GetHotWalletBalanceAsync())
                .Returns(Task.FromResult(currentHotWalletBalance));
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(pendingWithdrawals.AsReadOnly()));
            _timerHandler.Invoke();
            _stellarServiceMock.Verify(
                service => service.SubmitHotWalletWithdrawalAsync(
                    It.Is<string>(value => value == _stellarConfigurationService.Cold.Public),
                    It.Is<decimal>(value =>
                        value == currentHotWalletBalance - pendingWithdrawals.Sum(request => request.Amount) -
                        _stellarConfigurationService.HotWalletThreshold)));
        }

        [Test]
        public void ReallocationProcessor_Notifies_If_Failed_To_Submit_Transaction()
        {
            var currentHotWalletBalance = 20000m;
            var pendingWithdrawals = GetWithdrawalRequests();
            _stellarServiceMock.Setup(service => service.GetHotWalletBalanceAsync())
                .Returns(Task.FromResult(currentHotWalletBalance));
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(pendingWithdrawals.AsReadOnly()));

            _stellarServiceMock.Setup(service => service.SubmitHotWalletWithdrawalAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .Throws<NullReferenceException>();
            _timerHandler.Invoke();
            _emailServiceMock.Verify(service => service.Send(It.IsAny<ReallocationFailedEmailParameters>()));

            _stellarServiceMock.Setup(service => service.SubmitHotWalletWithdrawalAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .Throws<StellarServiceException>();
            _timerHandler.Invoke();
            _emailServiceMock.Verify(service => service.Send(It.IsAny<ReallocationFailedEmailParameters>()));
        }

        private List<WithdrawalRequest> GetWithdrawalRequests()
        {
            return new List<WithdrawalRequest>
            {
                new WithdrawalRequest
                {
                    Address = "address1",
                    Amount = 100m,
                    Id = 1,
                    TraderId = 1,
                    Status = WithdrwalRequestStatus.Pending
                },
                new WithdrawalRequest
                {
                    Address = "address2",
                    Amount = 200m,
                    Id = 2,
                    TraderId = 2,
                    Status = WithdrwalRequestStatus.Pending
                }
            };
        }
    }
}