#region Usings

using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Provider.Email
{
    public interface IEmailProvider : ITRiZHubProvider
    {
        void SendPasswordEmailToUser(UserIdentity user, string password);
    }
}