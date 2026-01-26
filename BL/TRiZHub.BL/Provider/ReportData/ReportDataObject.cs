#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ReportData
{
    public class ReportDataObject
    {
        public Guid? Id { get; set; }

        public bool Bool { get; set; }

        public string Value { get; set; }

        public ReportDataObject LinkedObject { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}