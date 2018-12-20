using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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

namespace Tradeio.Stellar.Tests
{
    public class DepositProcessorTests
    {
        private IStellarConfigurationService _stellarConfigurationService;
        private DepositProcessor _depositProcessor;
        private EventHandler<OperationResponse> _operationHandler;
        private Mock<IStellarClient> _stellarClientMock;
        private Mock<IStellarRepository> _stellarRepositoryMock;
        private Mock<IBalanceService> _balanceServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private readonly KeyPair _hotAccount = KeyPair.Random();
        private readonly KeyPair _traderAccount = KeyPair.Random();

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
            _stellarClientMock.Setup(client => client.ListenAccountOperations(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<EventHandler<OperationResponse>>()))
                .Callback<string, string, EventHandler<OperationResponse>>((accont, cursor, handler) =>
                    _operationHandler = handler);

            _depositProcessor = new DepositProcessor(
                _stellarConfigurationService,
                _stellarClientMock.Object,
                _stellarRepositoryMock.Object,
                _balanceServiceMock.Object,
                _emailServiceMock.Object);
        }

        [Test]
        public void DepositAddressService_Generates_New_Address()
        {
            _depositProcessor.Start();
            _stellarClientMock.Setup(client => client.GetTransaction(It.IsAny<string>()))
                .Returns(Task.FromResult(GetTransaction()));
            _operationHandler.Invoke(null, GetOperation());
            _emailServiceMock.Verify(service => );
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
    }
}