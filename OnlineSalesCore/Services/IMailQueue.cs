using System.Collections.Concurrent;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OnlineSalesCore.Services
{
    public interface IMailQueue
    {
        ConcurrentQueue<MailMessage> Queue { get; }
        void Enqueue(MailMessage m);
        bool TryDequeue(out MailMessage m);
        void Clear();
    }
}