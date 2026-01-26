#region Usings

using System;
using TRiZHub.BL.Context;

#endregion

namespace TRiZHub.BL.Test.DataConnections
{
    public interface ITestDataConnection : IDisposable
    {
        DataContext Context { get; set; }
        void TearDownDatabase();
    }
}