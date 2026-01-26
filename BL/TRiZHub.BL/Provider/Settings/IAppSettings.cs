#region Usings

#endregion

namespace TRiZHub.BL.Provider.Settings
{
    public interface IAppSettings
    {
        string EmailFromName { get; }
        string EmailFromAddress { get; }
        string AboutApp { get; }
    }
}