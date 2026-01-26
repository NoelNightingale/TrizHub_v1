#region Usings

using System;

#endregion

namespace TRiZHub.Models
{
    public class IdGridModel : GridModel
    {
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
    }
}