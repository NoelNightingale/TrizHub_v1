#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRiZHub.BL.Provider.WorkTeamData;

#endregion

namespace TRiZHub.BL.Test.Providers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EffectiveAllocationsTest
    {
        private static readonly DateTime Start = new DateTime(2026, 10, 1);
        private static readonly DateTime End = new DateTime(2026, 10, 15);

        private readonly Guid _client = Guid.NewGuid();
        private readonly Guid _project = Guid.NewGuid();
        private readonly Guid _subProject = Guid.NewGuid();
        private readonly Guid _otherSubProject = Guid.NewGuid();

        private EffectiveAllocations _allocations;

        [TestInitialize]
        public void Setup()
        {
            _allocations = new EffectiveAllocations();
        }

        private void Grant(Guid? clientId, Guid? projectId, Guid? subProjectId, DateTime start, DateTime? end, string team = "Alpha")
        {
            _allocations.TeamGrants.Add(new TeamAllocationGrant
            {
                WorkTeamId = Guid.NewGuid(),
                WorkTeamName = team,
                ClientId = clientId,
                ProjectId = projectId,
                SubProjectId = subProjectId,
                StartDate = start,
                EndDate = end
            });
        }

        [TestMethod]
        public void NothingAllocated_IsNotAllowed()
        {
            Assert.IsFalse(_allocations.Allows(_client, _project, null, Start));
            Assert.IsFalse(_allocations.Allows(_client, _project, _subProject, Start));
        }

        [TestMethod]
        public void DirectAllocations_AreUndated()
        {
            _allocations.DirectProjectIds.Add(_project);

            Assert.IsTrue(_allocations.Allows(_client, _project, null, new DateTime(2000, 1, 1)));
            Assert.IsTrue(_allocations.Allows(_client, _project, _subProject, new DateTime(2099, 1, 1)));
        }

        [TestMethod]
        public void DirectSubProject_DoesNotAllowTheProjectLine()
        {
            _allocations.DirectSubProjectIds.Add(_subProject);

            Assert.IsTrue(_allocations.Allows(_client, _project, _subProject, Start));
            Assert.IsFalse(_allocations.Allows(_client, _project, null, Start));
            Assert.IsFalse(_allocations.Allows(_client, _project, _otherSubProject, Start));
        }

        [TestMethod]
        public void TeamClientGrant_CoversAllProjectsOnlyWithinMembership()
        {
            Grant(_client, null, null, Start, End);

            Assert.IsTrue(_allocations.Allows(_client, _project, null, Start));
            Assert.IsTrue(_allocations.Allows(_client, _project, _subProject, End));
            Assert.IsFalse(_allocations.Allows(_client, _project, null, Start.AddDays(-1)));
            Assert.IsFalse(_allocations.Allows(_client, _project, null, End.AddDays(1)));
        }

        [TestMethod]
        public void OpenEndedTeamGrant_HasNoEnd()
        {
            Grant(null, _project, null, Start, null);

            Assert.IsTrue(_allocations.Allows(_client, _project, null, new DateTime(2099, 1, 1)));
            Assert.IsFalse(_allocations.Allows(_client, _project, null, Start.AddDays(-1)));
        }

        [TestMethod]
        public void TeamSubProjectGrant_OnlyThatSubProject()
        {
            Grant(null, _project, _subProject, Start, End);

            Assert.IsTrue(_allocations.Allows(_client, _project, _subProject, Start));
            Assert.IsFalse(_allocations.Allows(_client, _project, _otherSubProject, Start));
            Assert.IsFalse(_allocations.Allows(_client, _project, null, Start));
        }

        [TestMethod]
        public void UndatedQuery_MeansAnyDayInTheLoadedRange()
        {
            Grant(null, _project, null, Start, End);

            Assert.IsTrue(_allocations.WholeProjectCovered(_project));
            Assert.IsTrue(_allocations.Allows(_client, _project, null, null));
        }

        [TestMethod]
        public void TeamEndedButDirectRemains_StillAllowed()
        {
            Grant(null, _project, null, Start, End);
            _allocations.DirectClientIds.Add(_client);

            Assert.IsTrue(_allocations.Allows(_client, _project, null, End.AddDays(30)));
        }

        [TestMethod]
        public void TeamNamesFor_ListsDistinctTeams()
        {
            Grant(null, _project, null, Start, End, "Beta");
            Grant(null, _project, null, Start, End, "Alpha");
            Grant(null, _project, null, Start, End, "Alpha");
            Grant(_client, null, null, Start, End, "Gamma");

            CollectionAssert.AreEqual(new[] { "Alpha", "Beta" }, _allocations.TeamNamesFor(null, _project, null));
            CollectionAssert.AreEqual(new[] { "Gamma" }, _allocations.TeamNamesFor(_client, null, null));
        }
    }
}
