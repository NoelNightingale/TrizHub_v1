#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ClientEntityData
{
    public class ClientException : Exception
    {
        public ClientException(string error) : base(error)
        {
        }
    }
}