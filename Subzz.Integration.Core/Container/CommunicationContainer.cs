using Microsoft.AspNetCore.DataProtection;
using Subzz.Integration.Core.Notification;
using SubzzV2.Integration.Core.Notification;
using SubzzV2.Integration.Core.Notification.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Subzz.Integration.Core.Container
{
    public class CommunicationContainer
    {
        private ILogger _logger;
        public virtual ILogger Logger
        {
            get
            {
                return _logger ?? (_logger = new Logger());
            }
        }

        private MailTemplatesBuilder _mailTemplatesBuilder;

        public virtual MailTemplatesBuilder MailTemplatesBuilder
        {
            get
            {
                return _mailTemplatesBuilder ?? (_mailTemplatesBuilder = new MailTemplatesBuilder());
            }
        }

        private IEmailProcessor _emailProcessor;
        public virtual IEmailProcessor EmailProcessor
        {
            get
            {
                return _emailProcessor ?? (_emailProcessor = new EmailProcessor());
            }
        }

        private ISMSProcessor _smsProcessor;
        public virtual ISMSProcessor SMSProcessor
        {
            get
            {
                return _smsProcessor ?? (_smsProcessor = new SMSProcessor());
            }
        }

        private MailSettingProvider _mailSettingProvider;

        public virtual MailSettingProvider MailSettingProvider
        {
            get
            {
                return _mailSettingProvider ?? (_mailSettingProvider = new MailSettingProvider());
            }
        }

        private IEmailClient _mailClient;
        public virtual IEmailClient MailClient
        {
            get
            {
                return _mailClient ?? (_mailClient = new MailClient(MailSettingProvider.GetSetting()));
            }
        }

        private SmsMessagingSetting _smsMessagingSetting;

        public virtual SmsMessagingSetting SmsMessagingSetting
        {
            get
            {
                return _smsMessagingSetting ?? (_smsMessagingSetting = new SmsMessagingSetting());
            }
        }

        private IMessagingClient _messagingClient;
        public IMessagingClient MessagingClient
        {
            get
            {
                return _messagingClient ?? (_messagingClient = new MessagingClient(
                    new MessagingClientWrapper(SmsMessagingSetting.GetSetting())));
            }
        }
    }
}
