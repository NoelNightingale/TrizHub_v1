#region Usings

using System;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;

#endregion

namespace TRiZHub.Tests.Fake
{
    public class FakeAppSettings : TRiZHubProvider, IAppSettings
    {
        public FakeAppSettings(DataContext context) : base(context)
        {
        }

        public FakeAppSettings(DataContext context, ICurrentUser currentUser) : base(context, currentUser)
        {
        }

        public string EmailFromAddress
        {
            get { return "test@mail.com"; }
        }

        public string AboutApp { get; }
        public string TwitterURL { get; }

        public string ApplicationName
        {
            get { return "chAdmin"; }
        }

        public bool EnableSubscriberRegistration { get; }

        public Guid DailyQuizUniqueId
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime DailyQuizLastGenerated
        {
            get { throw new NotImplementedException(); }
        }

        public int DailyQuizTotalRecords
        {
            get { throw new NotImplementedException(); }
        }

        public int QuizGeneratorTimer
        {
            get { throw new NotImplementedException(); }
        }

        public string EmailFromName
        {
            get { return "from test mail server"; }
        }

        public string RegistrationNumber
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string VatNumber
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PhysicalAddressLine1
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PhysicalAddressLine2
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PhysicalAddressCity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PostalAddressLine1
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PostalAddressLine2
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PostalAddressCity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PostalCode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string CompanyName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PaymentUserId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PaymentPassword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PaymentEntityId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PaymentCurrencyId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PaymentHost
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}