#region Usings

using System;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.ClientModels
{
    public class ClientGridModel
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}