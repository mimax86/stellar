using System.Threading.Tasks;
using Tradeio.Balance;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;

namespace Tradeio.Stellar.Processors
{
    public class WithdrawalProcessor
    {
        private readonly IBalanceService _balanceService;
        private readonly IStellarRepository _stellarRepository;
        private readonly IStellarClient _stellarClient;
        private readonly IStellarConfigurationService _stellarConfigurationService;

        public WithdrawalProcessor(IBalanceService balanceService, IStellarRepository stellarRepository,
            IStellarClient stellarClient, IStellarConfigurationService stellarConfigurationService)
        {
            _balanceService = balanceService;
            _stellarRepository = stellarRepository;
            _stellarClient = stellarClient;
            _stellarConfigurationService = stellarConfigurationService;
        }

        public void Start()
        {
            Task.Run(() => { })
        }
    }
}