#region Usings

using TRiZHub.BL.Context;
using TRiZHub.DbPopulate.DbGen;

#endregion

namespace TRiZHub.DbPopulate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TRiZHubDummyDbGenerator.Instance().CreateDummyDatabase(new DataContext());
        }
    }
}