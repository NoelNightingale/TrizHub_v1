using System;

namespace TRiZHub.Models.ActivityModels
{
    public class ActivityEditModel 
    {
        public Guid? Id { get; set; }

        public string ActivityName { get; set; }

        public bool IsActive { get; set; }
    }
}