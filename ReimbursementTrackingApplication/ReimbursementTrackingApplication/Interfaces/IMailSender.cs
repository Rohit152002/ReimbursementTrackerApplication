using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IMailSender
    {
        void SendEmail(Message message);

    }
}
