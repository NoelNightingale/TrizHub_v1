#region Usings

using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRiZHub.BL.Provider.WorkTeamData;

#endregion

namespace TRiZHub.BL.Test.Providers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WorkGroupAccessRulesTest
    {
        [TestMethod]
        public void AdminEditsEveryLevel()
        {
            Assert.IsTrue(WorkGroupAccess.CanEditGroup(true));
            Assert.IsTrue(WorkGroupAccess.CanManageStructure(true, false));
            Assert.IsTrue(WorkGroupAccess.CanEditMembers(true, false, false, false, false));
            Assert.IsTrue(WorkGroupAccess.CanSeeGroup(true, false, false));
            Assert.IsTrue(WorkGroupAccess.CanSeeTeam(true, false, false));
        }

        [TestMethod]
        public void ManagerEditsBeneathTheGroupOnly()
        {
            Assert.IsFalse(WorkGroupAccess.CanEditGroup(false));
            Assert.IsTrue(WorkGroupAccess.CanManageStructure(false, true));
            Assert.IsTrue(WorkGroupAccess.CanEditMembers(false, true, false, false, false));
            Assert.IsTrue(WorkGroupAccess.CanSeeGroup(false, true, false));
            Assert.IsTrue(WorkGroupAccess.CanSeeTeam(false, true, false));
            Assert.IsFalse(WorkGroupAccess.CanDeleteGroup(false, 0, 0));
        }

        [TestMethod]
        public void LeadEditsMembersOnAnActiveTeamOnly()
        {
            Assert.IsFalse(WorkGroupAccess.CanManageStructure(false, false));
            Assert.IsTrue(WorkGroupAccess.CanEditMembers(false, false, true, true, true));
            Assert.IsFalse(WorkGroupAccess.CanEditMembers(false, false, true, true, false));
            Assert.IsFalse(WorkGroupAccess.CanEditMembers(false, false, true, false, true));
            Assert.IsFalse(WorkGroupAccess.CanSeeTeam(false, false, false));
            Assert.IsTrue(WorkGroupAccess.CanSeeTeam(false, false, true));
            Assert.IsTrue(WorkGroupAccess.CanSeeGroup(false, false, true));
        }

        [TestMethod]
        public void MemberCanViewTheGroupAndTheirTeamOnly()
        {
            Assert.IsTrue(WorkGroupAccess.CanSeeGroup(false, false, true));
            Assert.IsTrue(WorkGroupAccess.CanSeeTeam(false, false, true));
            Assert.IsFalse(WorkGroupAccess.CanEditGroup(false));
            Assert.IsFalse(WorkGroupAccess.CanManageStructure(false, false));
            Assert.IsFalse(WorkGroupAccess.CanEditMembers(false, false, false, true, true));
        }

        [TestMethod]
        public void OutsiderSeesNothing()
        {
            Assert.IsFalse(WorkGroupAccess.CanSeeGroup(false, false, false));
            Assert.IsFalse(WorkGroupAccess.CanEditMembers(false, false, false, true, true));
        }

        [TestMethod]
        public void DeleteOnlyWhenTheNodeIsEmpty()
        {
            Assert.IsTrue(WorkGroupAccess.CanDeleteGroup(true, 0, 0));
            Assert.IsFalse(WorkGroupAccess.CanDeleteGroup(true, 1, 0));
            Assert.IsFalse(WorkGroupAccess.CanDeleteGroup(true, 0, 1));
            Assert.IsTrue(WorkGroupAccess.CanDeleteTeam(true, 0));
            Assert.IsFalse(WorkGroupAccess.CanDeleteTeam(true, 2));
            Assert.IsFalse(WorkGroupAccess.CanDeleteTeam(false, 0));
        }
    }
}
