using System.Collections.Concurrent;
using System.Net.Mail;
using OnlineSalesCore.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OnlineSalesCore.Services
{
    public class MailQueue : IMailQueue
    {
        private readonly MailerOptions _options;
        private static readonly ConcurrentQueue<MailMessage> _queue;
        public ConcurrentQueue<MailMessage> Queue => _queue;
        static MailQueue()
        {
            _queue = new ConcurrentQueue<MailMessage>();
        }

        public MailQueue(IOptions<MailerOptions> options)
        {
            _options = options.Value;
        }
        public void Enqueue(MailMessage m)
        {
            if (m == null)
                throw new System.ArgumentNullException(nameof(m));
            _queue.Enqueue(m);
        }
        public void Clear()
        {
            _queue.Clear();
        }

        public bool TryDequeue(out MailMessage m)
        {
            return _queue.TryDequeue(out m);
        }
    }
}