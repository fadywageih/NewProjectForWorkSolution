using Shared.Dtos.User;

namespace ServicesAbstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto email);
    }
}
