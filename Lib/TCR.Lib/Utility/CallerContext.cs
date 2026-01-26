#region Usings

using System.ServiceModel;
using System.ServiceModel.Channels;

#endregion

namespace TCR.Lib.Utility
{
    public class CallerContext
    {
        public static string[] LocalIps = {"::1", "127.0.0.1"};

        public static string CallerIp
        {
            get
            {
                var context = OperationContext.Current;
                if (context == null)
                    return "::1";

                var messageProperties = context.IncomingMessageProperties;
                var endpointProperty =
                    messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return endpointProperty.Address;
            }
        }
    }
}