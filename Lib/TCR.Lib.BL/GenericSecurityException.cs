#region Usings

using System;

#endregion

namespace TCR.Lib.BL
{
    public class GenericSecurityException :
        Exception
    {
        public GenericSecurityException(string message) :
            base(message)
        {
        }
    }
}