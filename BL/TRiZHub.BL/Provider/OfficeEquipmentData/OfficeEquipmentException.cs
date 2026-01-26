#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.OfficeEquipmentData
{
    public class OfficeEquipmentException : Exception
    {
        public OfficeEquipmentException(string error) : base(error)
        {
        }
    }
}