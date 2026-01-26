#region Usings

using System;

#endregion

namespace TRiZHub.Models.StockTake
{
    public class StockTakeListItemModel
    {
        public Guid StockTakeId { get; set; }

        public string Description { get; set; }

        public int NumProducts { get; set; }
    }
}