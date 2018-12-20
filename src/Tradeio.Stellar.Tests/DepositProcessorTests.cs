using System;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;

namespace Tradeio.Stellar.Tests
{
    public class DepositProcessorTests
    {
        private IStellarConfigurationService _stellarConfigurationService;
        private DepositProcessor _depositProcessor;
        private Mock<IStellarRepository> _stellarRepositoryMock;
        private Mock<IBalanceService> _balanceServiceMock;
        private Mock<IEmailService> _emailServiceMock;

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

            _depositProcessor = new DepositProcessor(_stellarConfigurationService, _stellarRepositoryMock.Object,
                _balanceServiceMock.Object, _emailServiceMock.Object);
        }

        [Test]
        public void DepositAddressService_Generates_New_Address()
        {

        }
    }
}