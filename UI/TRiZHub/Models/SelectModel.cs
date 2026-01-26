#region Usings

using System;

#endregion

namespace TRiZHub.Models
{
    public class SelectModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? ImageDataId { get; set; }
    }
}