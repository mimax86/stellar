using System.Threading.Tasks;

namespace Tradeio.Stellar.Withdrawal.Controllers
{
    public class StatusControllerInternal : IStatusController
    {
        public async Task<Status> GetAsync()
        {
            return new Status();
        }
    }
}