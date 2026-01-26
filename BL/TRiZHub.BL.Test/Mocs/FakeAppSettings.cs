#region Usings

using System;
using TRiZHub.BL.Provider.Settings;

#endregion

namespace TRiZHub.BL.Test.Mocs
{
    public class FakeAppSettings : IAppSettings
    {
        public string AboutTRiZHub
        {
            get { return "Nothing usefull"; }
        }

        public string AboutApp { get; }

        public string TwitterURL
        {
            get { return "Nothing usefull"; }
        }

        public bool EnableSubscriberRegistration
        {
            get { return true; }
        }

        public string EmailFromAddress
        {
            get { return "Fake@mail.com"; }
        }

        public string EmailFromName
        {
            get { return "Fake"; }
        }

        public string ApplicationName
        {
            get { return "No Name"; }
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