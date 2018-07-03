
using OnlineSalesCore.Models;

namespace OnlineSalesCore.Services
{
    public interface IMailerService
    {
        //Online bill is available
        void MailOnlineBillAvailable(OnlineOrder order, string to, string billNumber, string[] cc);
        //New case assigned to user
        void MailNewAssign(OnlineOrder order, string assignTo, string[] cc);
        //Case reached final status
        void MailStageChanged(OnlineOrder order, string stage, string to, string[] cc);
        //POS not regconized
        // void MailInvalidPOS(OnlineOrder order, string[] emails);
        //Case not assignale due to schedule
        void MailNotAssignable(OnlineOrder order, string bdsEmail, string reason, string[] cc);
    }
}