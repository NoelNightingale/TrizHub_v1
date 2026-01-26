#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ActivityData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using System.Collections.Generic;
#endregion

namespace TRiZHub.BL.Provider.ActivityData
{
    public class ActivityProvider : TRiZHubProvider, IActivityProvider
    {
        #region Constructor
        IList<PrivilegeType> getTokens;

        public ActivityProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>();
            getTokens.Add(PrivilegeType.ActivityMaintenance);
            getTokens.Add(PrivilegeType.TimesheetCapture);
        }

        #endregion

        #region Activity

        public IQueryable<Activity> ActivityList()
        {
            AuthenticateList(getTokens);
            return DataContext.ActivitySet;
        }

        public Activity GetActivity(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.ActivitySet.FirstOrDefault(a => a.Id == id);
        }

        public Activity SaveActivity(Guid? id, string activityName, bool isActive)
        {
            Authenticate(PrivilegeType.ActivityMaintenance);

            var existing = DataContext.ActivitySet.FirstOrDefault(a => a.ActivityName == activityName && a.Id != id);
            if (existing != null)
                throw new ActivityException("An activity with this name already exists.");

            var record = DataContext.ActivitySet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new Activity();
                DataContext.ActivitySet.Add(record);
            }

            record.ActivityName = activityName;
            record.IsActive = isActive;

            DataContextSaveChanges();

            return record;
        }

        #endregion
    }
}