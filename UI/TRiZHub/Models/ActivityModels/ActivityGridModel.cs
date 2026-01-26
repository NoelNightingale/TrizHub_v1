using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRiZHub.Models.ActivityModels
{
    public class ActivityGridModel
    {
        public Guid? Id { get; set; }

        public string ActivityName { get; set; }

        public bool IsActive { get; set; }
    }
}