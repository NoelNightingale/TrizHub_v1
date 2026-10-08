#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRiZHub.BL.Provider.BillingRatesData;
using TRiZHub.BL.Provider.WorkTeamData;

#endregion

namespace TRiZHub.BL.Test.Providers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TeamRateScopeTest
    {
        private static readonly DateTime Start = new DateTime(2026, 1, 1);
        private static readonly DateTime End = new DateTime(2026, 6, 30);

        private readonly Guid _team = Guid.NewGuid();
        private readonly Guid _client = Guid.NewGuid();
        private readonly Guid _otherClient = Guid.NewGuid();
        private readonly Guid _project = Guid.NewGuid();
        private readonly Guid _otherProject = Guid.NewGuid();

        private List<WorkTeamTypePeriod> _periods;
        private Dictionary<Guid, WorkTeamScope> _scopes;

        [TestInitialize]
        public void Setup()
        {
            _periods = new List<WorkTeamTypePeriod>
            {
                new WorkTeamTypePeriod { WorkTeamId = _team, StartDate = Start, EndDate = End }
            };
            var scope = new WorkTeamScope();
            scope.ClientIds.Add(_client);
            scope.ProjectIds.Add(_project);
            _scopes = new Dictionary<Guid, WorkTeamScope> { { _team, scope } };
        }

        private bool Allows(Guid? clientId, Guid? projectId, Guid? projectClientId, DateTime start, DateTime end)
        {
            return TeamRateScope.Allows(_periods, _scopes, clientId, projectId, projectClientId, start, end);
        }

        [TestMethod]
        public void DefaultRate_NeverAllowed()
        {
            Assert.IsFalse(Allows(null, null, null, Start, End));
        }

        [TestMethod]
        public void ClientRate_NeedsClientOnTeam()
        {
            Assert.IsTrue(Allows(_client, null, null, Start, End));
            Assert.IsFalse(Allows(_otherClient, null, null, Start, End));
        }

        [TestMethod]
        public void ProjectRate_AllowedByProjectOrItsClient()
        {
            Assert.IsTrue(Allows(null, _project, _otherClient, Start, End));
            Assert.IsTrue(Allows(null, _otherProject, _client, Start, End));
            Assert.IsFalse(Allows(null, _otherProject, _otherClient, Start, End));
        }

        [TestMethod]
        public void RatePeriod_MustOverlapReach()
        {
            Assert.IsTrue(Allows(_client, null, null, End, End.AddYears(1)));
            Assert.IsFalse(Allows(_client, null, null, End.AddDays(1), End.AddYears(1)));
            Assert.IsFalse(Allows(_client, null, null, Start.AddYears(-1), Start.AddDays(-1)));
        }

        [TestMethod]
        public void OpenEndedReach_CoversFutureRates()
        {
            _periods[0].EndDate = null;
            Assert.IsTrue(Allows(_client, null, null, End.AddYears(5), End.AddYears(6)));
        }

        [TestMethod]
        public void NoReach_NothingAllowed()
        {
            _periods.Clear();
            Assert.IsFalse(Allows(_client, null, null, Start, End));
        }
    }
}
