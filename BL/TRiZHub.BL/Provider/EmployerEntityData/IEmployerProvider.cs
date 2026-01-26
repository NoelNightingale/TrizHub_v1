#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.EmployerData;
using TRiZHub.BL.Entities.SecurityData;

#endregion Usings

namespace TRiZHub.BL.Provider.ClientEntityData
{
    public interface IEmployerProvider : ITRiZHubProvider
    {
        IQueryable<Employer> EmployerList();

        Employer GetEmployer(Guid id);

        Dictionary<Guid, string> GetUserEmployer(List<Guid> userIds);

        Employer SaveEmployer(Employer model);

        int Activate(Guid id);

        int Deactivate(Guid id);

        int Delete(Guid id);
    }
}