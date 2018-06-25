using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using OnlineSalesCore.EFModel;
using OnlineSalesCore.Options;

namespace OnlineSalesCore.Service
{
    public class MailerService : IMailerService
    {
        private readonly MailerOptions _options;
        private readonly IHtmlComposer _composer;
        private readonly IMailQueue _queue;
        public MailerService(IOptions<MailerOptions> options, IHtmlComposer composer, IMailQueue queue)
        {
            _options = options.Value;
            _composer = composer;
            _queue = queue;
        }

        public void MailNewAssign(OnlineOrder order)
        {
            _composer.AppendText("p", $"New order from: {order.Name} CMND: {order.NatId}");
            _composer.AppendText("p", $"Assigned to: {order.AssignUser.Username}");
            var mail = new MailMessage()
            {
                From = CreateMailAddress(_options.Username),
                IsBodyHtml = true,
                Body = _composer.ToString(),
                Subject = "New online sale case"
            };
            foreach (var address in _options.Receivers.Select(r => CreateMailAddress(r)))
            {
                mail.CC.Add(address);
            }
            mail.To.Add(CreateMailAddress(order.AssignUser.Username));
            _queue.Enqueue(mail);
        }
        public void MailStageChanged(OnlineOrder order)
        {
            _composer.AppendText("p", $"Case {order.Name} CMND: {order.NatId}");
            _composer.AppendText("p", $"Contract number: {order.Induscontract}");
            _composer.AppendText("p", $"Status changed to: {order.Stage.Stage}");
            var mail = new MailMessage()
            {
                From = CreateMailAddress(_options.Username),
                IsBodyHtml = true,
                Body = _composer.ToString(),
                Subject = $"Case's {order.Induscontract} status changed"
            };
            foreach (var address in _options.Receivers.Select(r => CreateMailAddress(r)))
            {
                mail.CC.Add(address);
            }
            mail.To.Add(CreateMailAddress(order.AssignUser.Username));
            _queue.Enqueue(mail);
        }
        private MailAddress CreateMailAddress(string s)
        {
            if(!s.Contains("@"))
                return new MailAddress(s + _options.Suffix);
            return new MailAddress(s);
        }
    }
}