#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.ActivityData;

#endregion

namespace TRiZHub.BL.Provider.ActivityData
{
    public interface IActivityProvider : ITRiZHubProvider
    {
        #region Activity

        Activity SaveActivity(Guid? id, string activityName, bool isActive);

        Activity GetActivity(Guid id);

        IQueryable<Activity> ActivityList();

        #endregion
    }
}