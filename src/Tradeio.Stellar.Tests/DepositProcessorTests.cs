using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Email.Parameters;
using Tradeio.Stellar.Data;
using Tradeio.Stellar.Data.Model;
using Tradeio.Stellar.Processors;

namespace Tradeio.Stellar.Tests
{
    public class DepositProcessorTests
    {
        private DepositProcessor _depositProcessor;
        private EventHandler<OperationResponse> _operationHandler;
        private Mock<IStellarService> _stellarServiceMock;
        private Mock<IStellarRepository> _stellarRepositoryMock;
        private Mock<IBalanceService> _balanceServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private readonly KeyPair _traderAccount = KeyPair.Random();

        [SetUp]
        public void Setup()
        {
            _stellarRepositoryMock = new Mock<IStellarRepository>();
            _balanceServiceMock = new Mock<IBalanceService>();
            _emailServiceMock = new Mock<IEmailService>();
            _stellarRepositoryMock = new Mock<IStellarRepository>();
            _stellarServiceMock = new Mock<IStellarService>();
            _stellarServiceMock.Setup(client => client.ListenHotWallet(It.IsAny<string>(),
                    It.IsAny<EventHandler<OperationResponse>>()))
                .Callback<string, EventHandler<OperationResponse>>((cursor, handler) =>
                    _operationHandler = handler);

            _depositProcessor = new DepositProcessor(
                _stellarServiceMock.Object,
                _stellarRepositoryMock.Object,
                _balanceServiceMock.Object,
                _emailServiceMock.Object);
        }

        [Test]
        public void DepositProcessor_Skips_Not_Payment_Operation()
        {
            _depositProcessor.Start();
            _operationHandler.Invoke(null, GetOffer());
            _stellarServiceMock.Verify(service => service.GetTransactionAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DepositProcessor_Sends_Email_If_Trader_Not_Found()
        {
            _depositProcessor.Start();
            _stellarServiceMock.Setup(client => client.GetTransactionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetTransaction()));
            _operationHandler.Invoke(null, GetOperation());
            _emailServiceMock.Verify(service => service.Send(It.IsAny<UnregisteredTraderDepositEmailParameters>()), Times.Once);
        }

        [Test]
        public void DepositProcessor_Creates_Transaction_For_Trader_Payment()
        {
            var traderAddress = new TraderAddress {CustomerId = "customerID", TraderId = 1};

            _depositProcessor.Start();
            _stellarServiceMock.Setup(service => service.GetTransactionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetTransaction()));
            _stellarRepositoryMock.Setup(repository => repository.GetTraderAddressByCustomerIdAsync("customerId"))
                .Returns(Task.FromResult(traderAddress));
            _operationHandler.Invoke(null, GetOperation());
            _stellarRepositoryMock.Verify(repository =>
                repository.CreateTransactionAsync(It.Is<TraderAddress>(address => address == traderAddress),
                    It.Is<decimal>(value => value == 10)), Times.Once);
        }

        private TransactionResponse GetTransaction()
        {
            return new TransactionResponse(default(string), default(long), default(string), _traderAccount, "token",
                default(long), default(long), 1, default(string), default(string), default(string), "customerId", null);
        }

        private OperationResponse GetOperation()
        {
            return new PaymentOperationResponse("10", "native", "XML", "issuer", KeyPair.Random(), KeyPair.Random());
        }

        private AccountMergeOperationResponse GetOffer()
        {
            return new AccountMergeOperationResponse(KeyPair.Random(), KeyPair.Random());
        }
    }
}  