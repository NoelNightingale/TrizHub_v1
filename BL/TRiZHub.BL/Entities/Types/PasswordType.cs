#region Usings

using System;

#endregion

namespace TRiZHub.BL.Entities.Types
{
    [Serializable]
    public enum PasswordType
    {
        Invalid,
        OneTimeUse,
        Temporary,
        Old
    }
}