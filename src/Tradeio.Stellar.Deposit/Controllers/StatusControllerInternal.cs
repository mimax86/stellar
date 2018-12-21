using System.Threading.Tasks;

namespace Tradeio.Stellar.Deposit.Controllers
{
    public class StatusControllerInternal : IStatusController
    {
        public async Task<Status> GetAsync()
        {
            return new Status();
        }
    }
}