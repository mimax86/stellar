using System;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Tradeio.Stellar.Configuration;

namespace Tradeio.Stellar.Tests
{
    public class DepositAddressServiceTests
    {
        private IStellarConfigurationService _stellarConfigurationService;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            _stellarConfigurationService = new StellarConfigurationService(configuration);
        }

        [Test]
        public void DepositAddressService_Generates_New_Address()
        {
            var depositAddressService = new DepositAddressService(_stellarConfigurationService);
            var address = depositAddressService.GetNewAddress();
        }
    }
}