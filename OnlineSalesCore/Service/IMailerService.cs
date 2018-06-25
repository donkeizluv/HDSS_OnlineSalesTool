
using OnlineSalesCore.EFModel;

namespace OnlineSalesCore.Service
{
    public interface IMailerService
    {
       void MailNewAssign(OnlineOrder order);
       void MailStageChanged(OnlineOrder order);
    }
}