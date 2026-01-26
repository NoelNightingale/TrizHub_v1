#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TCR.Lib.Email;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.EmailData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Resources;

#endregion

namespace TRiZHub.BL.Provider.Email
{
    public class EmailProvider : TRiZHubProvider, IEmailProvider
    {
        private string Template
        {
            get { return "Template.html"; }
        }

        private string PasswordReset
        {
            get { return "PasswordReset.html"; }
        }

        public void SendPasswordEmailToUser(UserIdentity user, string password)
        {
            var template = PasswordReset;
            var email = ResourceManager.EmailTemplate(template);
            email = email.Replace("{{NAME}}", user.FirstName);
            email = email.Replace("{{NEWPASSWORD}}", password);
            SendHTMLEmail(user.AccountName, "TRiZHub – Registration password", email);
        }

        private void SendHTMLEmail(string sendMailTo, string subject, string emailContent, string ccAddress,
            Dictionary<string, byte[]> attachments = null)
        {
            var emailBody = ResourceManager.EmailTemplate(Template).Replace("{{CONTENT-GOES-HERE}}", emailContent);
            var email = new EmailQueue
            {
                Created = DateTime.UtcNow,
                Status = EmailStatusType.Pending,
                ToAddress = sendMailTo,
                Subject = subject,
                MessageBody = emailBody,
                CCAddress = ccAddress
            };

            if (attachments != null)
            {
                email.EmailAttachments = new List<EmailAttachment>();
                foreach (var key in attachments.Keys)
                {
                    email.EmailAttachments.Add(new EmailAttachment
                    {
                        EmailQueue = email,
                        EmailQueueId = email.Id,
                        FileName = key,
                        FileData = attachments[key]
                    });
                }
            }

            DataContext.EmailQueueSet.Add(email);
            DataContextSaveChanges();
        }

        #region CTor

        private IAppSettings AppSettings { get; }

        public EmailProvider(DataContext context, IAppSettings appSettings)
            : base(context, null)
        {
            AppSettings = appSettings;
        }

        public EmailProvider(DataContext context, ICurrentUser currentUser, IAppSettings appSettings)
            : base(context, currentUser)
        {
            AppSettings = appSettings;
        }

        #endregion

        #region Email Processing

        public void ProcessQueue()
        {
            try
            {
                var emailsToSend =
                    DataContext.EmailQueueSet.Where(a => a.Status == EmailStatusType.Pending)
                        .OrderBy(a => a.Created)
                        .Take(30)
                        .ToList();
                foreach (var email in emailsToSend)
                {
                    email.Status = EmailStatusType.Processing;
                    email.Processed = DateTime.UtcNow;
                }
                DataContextSaveChanges();

                foreach (var email in emailsToSend)
                {
                    try
                    {
                        List<MailAttachment> attachments = null;
                        if (email.EmailAttachments.Count > 0)
                            attachments =
                                email.EmailAttachments.ToList()
                                    .Select(a => new MailAttachment(a.FileData, a.FileName))
                                    .ToList();

                        EmailSender.SimpleHtmlSendMail(AppSettings.EmailFromAddress, AppSettings.EmailFromName,
                            email.ToAddress, email.CCAddress, email.Subject, email.MessageBody, attachments);
                        email.Status = EmailStatusType.Sent;
                    }
                    catch (Exception e)
                    {
                        email.SendAttempts = email.SendAttempts + 1;
                        email.Status = EmailStatusType.Failed;
                        email.SendError = e.Message;
                    }
                    email.Processed = DateTime.UtcNow;
                    DataContextSaveChanges();
                }
            }
            catch (Exception e)
            {
                SendCriticalError(e);
            }

            try
            {
                DataContext.EmailHouseKeeping();
            }
            catch (Exception e)
            {
                SendCriticalError(e);
            }
        }

        private void SendHTMLEmail(string sendMailTo, string subject, string emailContent)
        {
            var emailBody = ResourceManager.EmailTemplate(Template).Replace("{{CONTENT-GOES-HERE}}", emailContent);
            DataContext.EmailQueueSet.Add(new EmailQueue
            {
                Created = DateTime.UtcNow,
                Status = EmailStatusType.Pending,
                ToAddress = sendMailTo,
                Subject = subject,
                MessageBody = emailBody
            });
            DataContextSaveChanges();
        }

        #endregion
    }
}