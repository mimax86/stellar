namespace Tradeio.Email
{
    public interface IEmailService
    {
        void Send(EmailParameters parameters);
    }
}