
using OnlineSalesCore.EFModel;

namespace OnlineSalesCore.Service
{
    public interface IMailerService
    {
        //Online bill is available
        void MailOnlineBillAvailable(OnlineOrder order, string to, string billNumber, string[] cc);
        //New case assigned to user
        void MailNewAssign(OnlineOrder order, string assignTo, string[] cc);
        //Case reached final status
        void MailStageChanged(OnlineOrder order, string stage, string to, string[] cc);
    }
}