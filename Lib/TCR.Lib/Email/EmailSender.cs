#region Usings

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#endregion

namespace TCR.Lib.Email
{
    public static class EmailSender
    {
        public static bool IsUnitTestig
        {
            get
            {
                var theSetting = ConfigurationManager.AppSettings["IsUnitTesting"];
                if (!String.IsNullOrWhiteSpace(theSetting) && theSetting.ToLower() == "true")
                    return true;

                return false;
            }
        }

        public static void ExecuteHtmlSendMail(string fromAddress, string fromName, string toAddress,
            string ccAddress, string bodyText, string subject, List<MailAttachment> attachments,
            bool brLineBreaks = true, bool useEmailThread = true)
        {
            if (IsUnitTestig) //dont actually send the email in unit test mode..
                return;


            var mailMsg = new MailMessage();

            mailMsg.From = new MailAddress(fromAddress, fromName);
            if (toAddress.Contains(";"))
            {
                foreach (var email in toAddress.Split(";".ToCharArray()))
                {
                    if (!string.IsNullOrWhiteSpace(email) && email.Contains("@"))
                    {
                        mailMsg.To.Add(new MailAddress(email));
                    }
                }
            }
            else
            {
                mailMsg.To.Add(new MailAddress(toAddress));
            }
            if (ccAddress != null && ccAddress.Length > 1 && ccAddress.Contains("@"))
                mailMsg.CC.Add(new MailAddress(ccAddress));

            mailMsg.Subject = subject;
            mailMsg.BodyEncoding = Encoding.GetEncoding("utf-8");

            var plainView = AlternateView.CreateAlternateViewFromString
                (Regex.Replace(bodyText, @"<(.|\n)*?>", string.Empty), null, "text/plain");
            if (brLineBreaks)
                bodyText = bodyText.Replace("\n", "<br>");
            var htmlView = AlternateView.CreateAlternateViewFromString(bodyText, null, "text/html");

            mailMsg.AlternateViews.Add(plainView);
            mailMsg.AlternateViews.Add(htmlView);

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    var at = new Attachment(file.ContentAttachment, file.FileName);
                    mailMsg.Attachments.Add(at);
                }
            }
        
            if (useEmailThread)
                new MailerThread(mailMsg, attachments);
            else
                new MailSender().SendMail(mailMsg, attachments);
        }

        public static void SimpleHtmlSendMail(string fromAddress, string fromName, string toAdderss, string ccAddress, string subject,
        string bodyText, List<MailAttachment> attachments = null)
        {
            if (string.IsNullOrWhiteSpace(ccAddress))
                ccAddress = null;
            ExecuteHtmlSendMail(fromAddress, fromName, toAdderss, ccAddress, bodyText, subject, attachments, false, false);
        }


        public static void SimpleHtmlSendMail(string fromAddress, string fromName, string toAdderss, string subject,
            string bodyText)
        {
            ExecuteHtmlSendMail(fromAddress, fromName, toAdderss, null, bodyText, subject, null, false, false);
        }

        public static void ExecuteHtmlSendMail(string fromAddress, string fromName, string toAddress, string bodyText,
            string subject, List<MailAttachment> attachments)
        {
            ExecuteHtmlSendMail(fromAddress, fromName, toAddress, "", bodyText, subject, attachments);
        }

        public static void ExecuteHtmlSendMail(string fromAddress, string fromName, string toAddress, string bodyText,
            string subject, MailAttachment attachment)
        {
            var attchlist = new List<MailAttachment>();
            attchlist.Add(attachment);
            ExecuteHtmlSendMail(fromAddress, fromName, toAddress, "", bodyText, subject, attchlist);
        }

        public static void ExecuteHtmlSendMail(string fromAddress, string fromName, string toAddress, string bodyText,
            string subject)
        {
            ExecuteHtmlSendMail(fromAddress, fromName, toAddress, "", bodyText, subject, null);
        }
    }

    public class MailerThread : IDisposable
    {
        private readonly List<MailAttachment> _attList;
        private readonly MailMessage _message;

        public MailerThread(MailMessage message, List<MailAttachment> attachments)
        {
            _message = message;
            _attList = attachments;
            new Thread(Run).Start();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _message.Dispose();
        }

        #endregion

        public void Run()
        {
            try
            {
                var smtp = new SmtpClient();
                smtp.Timeout = 200000;
                using (smtp)
                {
                    smtp.Send(_message);
                    if (_attList != null)
                    {
                        foreach (var o in _attList)
                            o.Dispose();

                        foreach (var a in _message.Attachments)
                        {
                            a.Dispose();
                        }
                    }
                }
            }
            catch
            {
            }

            Dispose();
        }
    }

    public class MailSender
    {
        internal void SendMail(MailMessage mailMsg, List<MailAttachment> attachments)
        {
            using (mailMsg)
            {
                using (var smtp = new SmtpClient {Timeout = 200000})
                {
                    smtp.Send(mailMsg);
                    if (attachments != null)
                    {
                        foreach (var o in attachments)
                            o.Dispose();

                        foreach (var a in mailMsg.Attachments)
                        {
                            a.Dispose();
                        }
                    }
                }
            }
        }
    }
}