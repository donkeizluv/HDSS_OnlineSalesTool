using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using OnlineSalesCore.Models;
using OnlineSalesCore.Options;

namespace OnlineSalesCore.Services
{
    public class MailerService : IMailerService
    {
        private readonly MailerOptions _options;
        private readonly IHtmlComposer _composer;
        private readonly IMailQueue _queue;
        public MailerService(IOptions<MailerOptions> options,
            IHtmlComposer composer,
            IMailQueue queue)
        {
            _options = options.Value;
            _composer = composer;
            _queue = queue;
        }
        public void MailNotAssignable(OnlineOrder order, string bdsEmail, string reason, string[] cc)
        {
            #region check
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (string.IsNullOrEmpty(bdsEmail))
            {
                throw new ArgumentException("message", nameof(bdsEmail));
            }
            #endregion
            _composer.Reset();
            _composer.AppendText("p", $"Cant assign case from: {order.Name} CMND: {order.NatId}");
            _composer.AppendText("p", "Please assign manually.");
            if(!string.IsNullOrEmpty(reason))
                _composer.AppendText("p", reason);
            var mail = new MailMessage()
            {
                From = CreateMailAddress(_options.Username),
                IsBodyHtml = true,
                Body = _composer.ToString(),
                Subject = "Can not assign online sale case"
            };
            //Always CC
            AddOptionsCC(mail);
            //Additional CC
            AddCC(mail, cc);

            mail.To.Add(CreateMailAddress(bdsEmail));
            _queue.Enqueue(mail);
        }
        //TODO: too compicated and floated to cache mailing list
        //Use mail group instead
        // public void MailInvalidPOS(OnlineOrder order, string[] emails)
        // {
        //     #region check
        //     if (order == null)
        //     {
        //         throw new ArgumentNullException(nameof(order));
        //     }

        //     if (emails == null)
        //     {
        //         throw new ArgumentException("message", nameof(emails));
        //     }
        //     #endregion
        //     _composer.Reset();
        //     _composer.AppendText("p", $"POS {order.PosCode} is not registered on the system.");
        //     _composer.AppendText("p", "Please assign manually.");
        //     var mail = new MailMessage()
        //     {
        //         From = CreateMailAddress(_options.Username),
        //         IsBodyHtml = true,
        //         Body = _composer.ToString(),
        //         Subject = "POS code is not available"
        //     };
        //     //Always CC
        //     AddOptionsCC(mail);
        //     //Additional CC
        //     AddTo(mail, emails);
        //     _queue.Enqueue(mail);
        // }
        public void MailNewAssign(OnlineOrder order, string assignTo, string[] cc)
        {
            #region check
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (string.IsNullOrEmpty(assignTo))
            {
                throw new ArgumentException("message", nameof(assignTo));
            }
            #endregion
            _composer.Reset();
            _composer.AppendText("p", $"New order from: {order.Name} CMND: {order.NatId}");
            _composer.AppendText("p", $"Assigned to: {assignTo}");
            var mail = new MailMessage()
            {
                From = CreateMailAddress(_options.Username),
                IsBodyHtml = true,
                Body = _composer.ToString(),
                Subject = "New online sale case assignment"
            };
            //Always CC
            AddOptionsCC(mail);
            //Additional CC
            AddCC(mail, cc);

            mail.To.Add(CreateMailAddress(assignTo));
            _queue.Enqueue(mail);
        }
        public void MailOnlineBillAvailable(OnlineOrder order, string to, string billNumber, string[] cc)
        {
            #region check
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("message", nameof(to));
            }

            if (string.IsNullOrEmpty(billNumber))
            {
                throw new ArgumentException("message", nameof(billNumber));
            }
            #endregion
            _composer.Reset();
            _composer.AppendText("p", $"Case {order.Name} CMND: {order.NatId}");
            _composer.AppendText("p", $"Contract number: {order.Induscontract ?? "null"}");
            _composer.AppendText("p", $"Bill number: {billNumber}");
            var mail = new MailMessage()
            {
                From = CreateMailAddress(_options.Username),
                IsBodyHtml = true,
                Body = _composer.ToString(),
                Subject = $"Case {order.Induscontract} is ready"
            };
            //Always CC
            AddOptionsCC(mail);
            //Additional CC
            AddCC(mail, cc);
            mail.To.Add(CreateMailAddress(to));
            _queue.Enqueue(mail);

        }
        public void MailStageChanged(OnlineOrder order, string stage, string to, string[] cc)
        {
            #region check
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (string.IsNullOrEmpty(stage))
            {
                throw new ArgumentException("message", nameof(stage));
            }

            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("message", nameof(to));
            }
            #endregion
            _composer.Reset();
            _composer.AppendText("p", $"Case {order.Name} CMND: {order.NatId}");
            _composer.AppendText("p", $"Contract number: {order.Induscontract ?? "null"}");
            _composer.AppendText("p", $"Status changed to: {stage}");
            var mail = new MailMessage()
            {
                From = CreateMailAddress(_options.Username),
                IsBodyHtml = true,
                Body = _composer.ToString(),
                Subject = $"Case {order.Induscontract} status changed"
            };
            //Always CC
            AddOptionsCC(mail);
            //Additional CC
            AddCC(mail, cc);
            mail.To.Add(CreateMailAddress(to));
            _queue.Enqueue(mail);
        }
        private void AddOptionsCC(MailMessage mail)
        {
            foreach (var address in _options.Receivers.Select(r => CreateMailAddress(r)))
            {
                mail.CC.Add(address);
            }
        }
        private void AddCC(MailMessage mail, string[] cc)
        {
            if (cc == null) return;
            foreach (var address in cc.Select(r => CreateMailAddress(r)))
            {
                mail.CC.Add(address);
            }
        }
         private void AddTo(MailMessage mail, string[] to)
        {
            if (to == null) return;
            foreach (var address in to.Select(r => CreateMailAddress(r)))
            {
                mail.CC.Add(address);
            }
        }
        private MailAddress CreateMailAddress(string s)
        {
            if (!s.Contains("@"))
                return new MailAddress(s + _options.Suffix);
            return new MailAddress(s);
        }
    }
}