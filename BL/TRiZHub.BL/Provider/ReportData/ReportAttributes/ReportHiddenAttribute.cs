#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ReportData.ReportAttributes
{
    public class ReportHiddenColumn : Attribute
    {
        public ReportHiddenColumn(int columnNumber)
        {
            ColumnNumber = columnNumber;
        }

        public int ColumnNumber { get; }
    }
}