#region Usings

using System;

#endregion

namespace TRiZHub.Models
{
    public class CheckListItemModel
    {
        public Guid Id { get; set; }
        public bool Selected { get; set; }
        public string Name { get; set; }
    }
}