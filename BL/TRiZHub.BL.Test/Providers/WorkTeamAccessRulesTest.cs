#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.WorkTeamData;
using TRiZHub.BL.Provider.WorkTeamData;

#endregion

namespace TRiZHub.BL.Test.Providers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WorkTeamAccessRulesTest
    {
        private static readonly DateTime Today = new DateTime(2026, 10, 8);

        private readonly Guid _manager = Guid.NewGuid();
        private readonly Guid _lead = Guid.NewGuid();
        private readonly Guid _member = Guid.NewGuid();
        private readonly Guid _endedMember = Guid.NewGuid();
        private readonly Guid _futureMember = Guid.NewGuid();
        private readonly Guid _outsider = Guid.NewGuid();

        private WorkTeam _team;
        private List<WorkTeamMember> _members;

        [TestInitialize]
        public void Setup()
        {
            _team = new WorkTeam { Name = "Alpha", IsActive = true, TeamTypeId = Guid.NewGuid() };
            _members = new List<WorkTeamMember>
            {
                Row(_team, _manager, WorkTeamRoleType.Manager, Today.AddYears(-1), null),
                Row(_team, _lead, WorkTeamRoleType.Lead, Today.AddYears(-1), null),
                Row(_team, _member, WorkTeamRoleType.Member, Today.AddMonths(-6), null),
                Row(_team, _endedMember, WorkTeamRoleType.Member, Today.AddMonths(-6), Today.AddDays(-10)),
                Row(_team, _futureMember, WorkTeamRoleType.Member, Today.AddDays(10), null)
            };
        }

        private static WorkTeamMember Row(WorkTeam team, Guid user, WorkTeamRoleType role, DateTime start, DateTime? end)
        {
            return new WorkTeamMember
            {
                WorkTeam = team,
                WorkTeamId = team.Id,
                UserAccountId = user,
                Role = role,
                StartDate = start,
                EndDate = end
            };
        }

        private List<Guid> Reach(Guid actor, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return WorkTeamAccessRules.ReachableUserIds(_members.AsQueryable(), actor, cap, Today, from, to).ToList();
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void TimesheetFlagOff_NoOneReaches()
        {
            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Timesheets, Today, Today).Count);
            Assert.AreEqual(0, Reach(_lead, WorkTeamCapability.Timesheets, Today, Today).Count);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void TimesheetFlagOn_ManagerReachesMembersAndLeads_LeadReachesMembersOnly()
        {
            _team.AllowTeamTimesheets = true;

            var managerReach = Reach(_manager, WorkTeamCapability.Timesheets, Today, Today);
            CollectionAssert.Contains(managerReach, _member);
            CollectionAssert.Contains(managerReach, _lead);
            CollectionAssert.DoesNotContain(managerReach, _manager);

            var leadReach = Reach(_lead, WorkTeamCapability.Timesheets, Today, Today);
            CollectionAssert.Contains(leadReach, _member);
            CollectionAssert.DoesNotContain(leadReach, _lead);
            CollectionAssert.DoesNotContain(leadReach, _manager);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void ScorecardFlag_IsIndependentOfTimesheetFlag()
        {
            _team.AllowTeamTimesheets = true;
            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Scorecards, Today, Today).Count);

            _team.AllowTeamScorecards = true;
            CollectionAssert.Contains(Reach(_lead, WorkTeamCapability.Scorecards, Today, Today), _member);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void Rates_ManagerAlways_LeadOnlyWithFlag()
        {
            CollectionAssert.Contains(Reach(_manager, WorkTeamCapability.Rates, Today, Today), _member);
            Assert.AreEqual(0, Reach(_lead, WorkTeamCapability.Rates, Today, Today).Count);

            _team.LeadsManageRates = true;
            CollectionAssert.Contains(Reach(_lead, WorkTeamCapability.Rates, Today, Today), _member);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void TeamAllocationsAndManageTeam_FollowRoleAndFlag()
        {
            var editable = WorkTeamAccessRules.QualifyingActorRows(_members.AsQueryable(), _lead, WorkTeamCapability.TeamAllocations, Today);
            Assert.AreEqual(0, editable.Count());

            _team.LeadsManageAllocations = true;
            editable = WorkTeamAccessRules.QualifyingActorRows(_members.AsQueryable(), _lead, WorkTeamCapability.TeamAllocations, Today);
            Assert.AreEqual(1, editable.Count());

            Assert.AreEqual(0, WorkTeamAccessRules.QualifyingActorRows(_members.AsQueryable(), _lead, WorkTeamCapability.ManageTeam, Today).Count());
            Assert.AreEqual(1, WorkTeamAccessRules.QualifyingActorRows(_members.AsQueryable(), _manager, WorkTeamCapability.ManageTeam, Today).Count());
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void TargetPeriodMustCoverRecordDate()
        {
            _team.AllowTeamTimesheets = true;

            CollectionAssert.DoesNotContain(Reach(_manager, WorkTeamCapability.Timesheets, Today, Today), _endedMember);
            CollectionAssert.Contains(Reach(_manager, WorkTeamCapability.Timesheets, Today.AddDays(-10), Today.AddDays(-10)), _endedMember);
            CollectionAssert.DoesNotContain(Reach(_manager, WorkTeamCapability.Timesheets, Today.AddDays(-9), Today.AddDays(-9)), _endedMember);

            CollectionAssert.DoesNotContain(Reach(_manager, WorkTeamCapability.Timesheets, Today, Today), _futureMember);
            CollectionAssert.Contains(Reach(_manager, WorkTeamCapability.Timesheets, Today.AddDays(10), Today.AddDays(10)), _futureMember);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void ActorMustHoldRoleToday()
        {
            _team.AllowTeamTimesheets = true;
            _members.Single(m => m.UserAccountId == _manager).EndDate = Today.AddDays(-1);

            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Timesheets, Today.AddMonths(-1), Today.AddMonths(-1)).Count);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void InactiveTeam_GrantsNothing()
        {
            _team.AllowTeamTimesheets = true;
            _team.IsActive = false;

            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Timesheets, Today, Today).Count);
            Assert.AreEqual(0, WorkTeamAccessRules.OwnRowsOverlapping(_members.AsQueryable(), _member, Today, Today).Count());
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void MultipleTeams_ReachOnlyThroughQualifyingTeam()
        {
            var other = new WorkTeam { Name = "Beta", IsActive = true, AllowTeamTimesheets = false };
            _members.Add(Row(other, _manager, WorkTeamRoleType.Manager, Today.AddYears(-1), null));
            _members.Add(Row(other, _outsider, WorkTeamRoleType.Member, Today.AddYears(-1), null));
            _team.AllowTeamTimesheets = true;

            var reach = Reach(_manager, WorkTeamCapability.Timesheets, Today, Today);
            CollectionAssert.Contains(reach, _member);
            CollectionAssert.DoesNotContain(reach, _outsider);

            var teams = WorkTeamAccessRules.ReachingTeamIds(_members.AsQueryable(), _manager, _member, WorkTeamCapability.Rates, Today, Today, Today).ToList();
            CollectionAssert.AreEqual(new List<Guid> { _team.Id }, teams);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void OwnRows_IncludeEveryRole()
        {
            Assert.AreEqual(1, WorkTeamAccessRules.OwnRowsOverlapping(_members.AsQueryable(), _manager, Today, Today).Count());
            Assert.AreEqual(1, WorkTeamAccessRules.OwnRowsOverlapping(_members.AsQueryable(), _lead, Today, Today).Count());
            Assert.AreEqual(0, WorkTeamAccessRules.OwnRowsOverlapping(_members.AsQueryable(), _endedMember, Today, Today).Count());
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void Overlaps_TreatsNullEndAsOpen()
        {
            Assert.IsTrue(WorkTeamAccessRules.Overlaps(Today, null, Today.AddYears(5), Today.AddYears(5)));
            Assert.IsTrue(WorkTeamAccessRules.Overlaps(Today, Today, Today, Today));
            Assert.IsFalse(WorkTeamAccessRules.Overlaps(Today, Today, Today.AddDays(1), Today.AddDays(2)));
            Assert.IsFalse(WorkTeamAccessRules.Overlaps(Today.AddDays(1), null, Today, Today));
        }
    }
}
