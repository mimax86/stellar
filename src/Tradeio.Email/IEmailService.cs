using Tradeio.Email.Parameters;

namespace Tradeio.Email
{
    public interface IEmailService
    {
        void Send(EmailParameters parameters);
    }
}