using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;
using Tradeio.Stellar.Data.Model;
using Tradeio.Stellar.Processors;
using Tradeio.Stellar.Processors.Timing;

namespace Tradeio.Stellar.Tests
{
    public class WithdrawalProcessorTests
    {
        private IStellarConfigurationService _stellarConfigurationService;
        private WithdrawalProcessor _withdrawalProcessor;
        private EventHandler<OperationResponse> _operationHandler;
        private Mock<IStellarClient> _stellarClientMock;
        private Mock<IStellarRepository> _stellarRepositoryMock;
        private Mock<IBalanceService> _balanceServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private readonly KeyPair _hotAccount = KeyPair.Random();
        private readonly KeyPair _traderAccount = KeyPair.Random();
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
            _stellarClientMock = new Mock<IStellarClient>();
            _stellarClientMock.Setup(client => client.ListenHotWallet(It.IsAny<string>(),
                    It.IsAny<EventHandler<OperationResponse>>()))
                .Callback<string, EventHandler<OperationResponse>>((cursor, handler) =>
                    _operationHandler = handler);
            var timerFactoryMock = new Mock<ITimerFactory>();
            timerFactoryMock.Setup(factory => factory.Create(It.IsAny<Action>()))
                .Returns(new Mock<ITimer>().Object)
                .Callback<Action>(handler => _timerHandler = handler);
            _withdrawalProcessor = new WithdrawalProcessor(
                _balanceServiceMock.Object,
                _stellarRepositoryMock.Object,
                _stellarClientMock.Object,
                _stellarConfigurationService,
                timerFactoryMock.Object,
                _emailServiceMock.Object,
                new NullLoggerFactory());
        }

        [Test]
        public void WithdrawalProcessor_Changes_Withdrawal_Status_To_Processing()
        {
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(new List<WithdrawalRequest> { GetWithdrawalequest() }.AsReadOnly()));
            _timerHandler.Invoke();
            _stellarRepositoryMock.Verify(repository =>
                repository.ChangeWithdrawalRequestStatus(It.IsAny<WithdrawalRequest>(),
                    It.Is<WithdrwalRequestStatus>(status => status == WithdrwalRequestStatus.Processing)));
        }

        [Test]
        public void WithdrawalProcessor_Sends_Email_Notification_If_Trader_Balance_Is_Insufficient()
        {
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(new List<WithdrawalRequest> {GetWithdrawalequest()}.AsReadOnly()));
            _balanceServiceMock
                .Setup(service => service.GetBalance(It.Is<long>(value => value == 1), It.IsAny<string>()))
                .Returns(1m);
            _timerHandler.Invoke();
            _emailServiceMock.Verify(service => service.Send(It.IsAny<EmailParameters>()), Times.Once);
        }

        [Test]
        public void WithdrawalProcessor_Sends_Email_Notification_If_Hot_Wallet_Balance_Is_Insufficient()
        {
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(new List<WithdrawalRequest> { GetWithdrawalequest() }.AsReadOnly()));
            _balanceServiceMock
                .Setup(service => service.GetBalance(It.Is<long>(value => value == 1), It.IsAny<string>()))
                .Returns(1000m);
            _stellarClientMock.Setup(client => client.GetHotWalletBalance())
                .Returns(10m);
            _timerHandler.Invoke();
            _emailServiceMock.Verify(service => service.Send(It.IsAny<EmailParameters>()), Times.Once);
        }

        [Test]
        public void WithdrawalProcessor_Changes_Request_Status_To_Error_If_Exception_Happens()
        {
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(new List<WithdrawalRequest> { GetWithdrawalequest() }.AsReadOnly()));
            _balanceServiceMock
                .Setup(service => service.GetBalance(It.Is<long>(value => value == 1), It.IsAny<string>()))
                .Returns(1000m);
            _stellarClientMock.Setup(client => client.GetHotWalletBalance())
                .Returns(1000m);
            _stellarClientMock.Setup(client => client.SubmitPayment(It.IsAny<string>(), It.IsAny<decimal>()))
                .Throws<StellarClientException>();
            _timerHandler.Invoke();
            _stellarRepositoryMock.Verify(repository =>
                repository.ChangeWithdrawalRequestStatus(It.IsAny<WithdrawalRequest>(),
                    It.Is<WithdrwalRequestStatus>(status => status == WithdrwalRequestStatus.Error)));
        }

        [Test]
        public void WithdrawalProcessor_Changes_Request_Status_To_Completed()
        {
            _stellarRepositoryMock.Setup(repository => repository.GetPendingWithdrawalRequestsAsync())
                .Returns(Task.FromResult(new List<WithdrawalRequest> { GetWithdrawalequest() }.AsReadOnly()));
            _balanceServiceMock
                .Setup(service => service.GetBalance(It.Is<long>(value => value == 1), It.IsAny<string>()))
                .Returns(1000m);
            _stellarClientMock.Setup(client => client.GetHotWalletBalance())
                .Returns(1000m);
            _timerHandler.Invoke();
            _stellarRepositoryMock.Verify(repository =>
                repository.ChangeWithdrawalRequestStatus(It.IsAny<WithdrawalRequest>(),
                    It.Is<WithdrwalRequestStatus>(status => status == WithdrwalRequestStatus.Completed)));
        }

        private WithdrawalRequest GetWithdrawalequest()
        {
            return new WithdrawalRequest
            {
                Address = "address",
                Amount = 100.23m,
                Id = 1,
                TraderId = 1,
                Status = WithdrwalRequestStatus.Pending
            };
        }
    }
}