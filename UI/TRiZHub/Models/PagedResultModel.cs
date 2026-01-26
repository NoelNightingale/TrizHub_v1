#region Usings

using System.Collections.Generic;

#endregion

namespace TRiZHub.Models
{
    public class PagedResultModel<T>
    {
        public PagedResultModel(List<T> data, int totalRecords)
        {
            Results = data;
            RecordCount = totalRecords;
        }

        public List<T> Results { get; set; }
        public int RecordCount { get; set; }
    }
}