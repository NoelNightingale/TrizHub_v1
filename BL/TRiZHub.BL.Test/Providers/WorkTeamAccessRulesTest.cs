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
        private readonly Guid _otherMember = Guid.NewGuid();
        private readonly Guid _endedMember = Guid.NewGuid();
        private readonly Guid _futureMember = Guid.NewGuid();
        private readonly Guid _outsider = Guid.NewGuid();

        private WorkTeam _team;
        private List<WorkTeamMember> _members;
        private List<WorkTeamRolePrivilege> _defaults;
        private List<WorkTeamMemberPrivilege> _overrides;

        [TestInitialize]
        public void Setup()
        {
            _team = new WorkTeam { Name = "Alpha", IsActive = true, TeamTypeId = Guid.NewGuid() };
            _members = new List<WorkTeamMember>
            {
                Row(_team, _manager, WorkTeamRoleType.Manager, Today.AddYears(-1), null),
                Row(_team, _lead, WorkTeamRoleType.Lead, Today.AddYears(-1), null),
                Row(_team, _member, WorkTeamRoleType.Member, Today.AddMonths(-6), null),
                Row(_team, _otherMember, WorkTeamRoleType.Member, Today.AddMonths(-6), null),
                Row(_team, _endedMember, WorkTeamRoleType.Member, Today.AddMonths(-6), Today.AddDays(-10)),
                Row(_team, _futureMember, WorkTeamRoleType.Member, Today.AddDays(10), null)
            };
            _defaults = new List<WorkTeamRolePrivilege>();
            _overrides = new List<WorkTeamMemberPrivilege>();
            foreach (WorkTeamRoleType role in Enum.GetValues(typeof(WorkTeamRoleType)))
                Default(role, WorkTeamPrivilege.CaptureTime, true);
            Default(WorkTeamRoleType.Manager, WorkTeamPrivilege.ManageRates, true);
            Default(WorkTeamRoleType.Manager, WorkTeamPrivilege.ManageMembers, true);
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

        private void Default(WorkTeamRoleType role, WorkTeamPrivilege privilege, bool granted)
        {
            _defaults.Add(new WorkTeamRolePrivilege { Role = role, Privilege = privilege, IsGranted = granted });
        }

        private void Override(Guid user, WorkTeamPrivilege privilege, bool granted)
        {
            var member = _members.Single(m => m.UserAccountId == user && m.WorkTeamId == _team.Id);
            _overrides.Add(new WorkTeamMemberPrivilege
            {
                WorkTeamMemberId = member.Id,
                Privilege = privilege,
                IsGranted = granted
            });
        }

        private List<Guid> Reach(Guid actor, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return WorkTeamAccessRules.ReachableUserIds(_members.AsQueryable(), _defaults.AsQueryable(), _overrides.AsQueryable(),
                actor, cap, Today, from, to).ToList();
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void TeamTimesheets_DefaultOff_NoOneReaches()
        {
            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Timesheets, Today, Today).Count);
            Assert.AreEqual(0, Reach(_lead, WorkTeamCapability.Timesheets, Today, Today).Count);
            Assert.AreEqual(0, Reach(_member, WorkTeamCapability.Timesheets, Today, Today).Count);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void TeamTimesheets_Override_ManagerReachesLeadsAndMembers_LeadReachesMembers_MemberReachesMembers()
        {
            Override(_manager, WorkTeamPrivilege.TeamTimesheets, true);
            Override(_lead, WorkTeamPrivilege.TeamTimesheets, true);
            Override(_member, WorkTeamPrivilege.TeamTimesheets, true);

            var managerReach = Reach(_manager, WorkTeamCapability.Timesheets, Today, Today);
            CollectionAssert.Contains(managerReach, _member);
            CollectionAssert.Contains(managerReach, _otherMember);
            CollectionAssert.Contains(managerReach, _lead);
            CollectionAssert.DoesNotContain(managerReach, _manager);

            var leadReach = Reach(_lead, WorkTeamCapability.Timesheets, Today, Today);
            CollectionAssert.Contains(leadReach, _member);
            CollectionAssert.DoesNotContain(leadReach, _lead);
            CollectionAssert.DoesNotContain(leadReach, _manager);

            var memberReach = Reach(_member, WorkTeamCapability.Timesheets, Today, Today);
            CollectionAssert.Contains(memberReach, _otherMember);
            CollectionAssert.DoesNotContain(memberReach, _member);
            CollectionAssert.DoesNotContain(memberReach, _lead);
            CollectionAssert.DoesNotContain(memberReach, _manager);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void MissingOverride_UsesRoleDefault()
        {
            CollectionAssert.Contains(Reach(_manager, WorkTeamCapability.Rates, Today, Today), _member);
            Assert.AreEqual(0, Reach(_lead, WorkTeamCapability.Rates, Today, Today).Count);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void OverrideDeny_BeatsRoleDefault()
        {
            Override(_manager, WorkTeamPrivilege.ManageRates, false);
            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Rates, Today, Today).Count);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void Scorecards_AreIndependentOfTimesheets()
        {
            Override(_manager, WorkTeamPrivilege.TeamTimesheets, true);
            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Scorecards, Today, Today).Count);

            Override(_lead, WorkTeamPrivilege.TeamScorecards, true);
            CollectionAssert.Contains(Reach(_lead, WorkTeamCapability.Scorecards, Today, Today), _member);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void ManageMembers_IsTheManagerDefault_AndGatesAllocations()
        {
            Assert.AreEqual(1, WorkTeamAccessRules.QualifyingActorRows(_members.AsQueryable(), _defaults.AsQueryable(),
                _overrides.AsQueryable(), _manager, WorkTeamCapability.ManageTeam, Today).Count());
            Assert.AreEqual(0, WorkTeamAccessRules.QualifyingActorRows(_members.AsQueryable(), _defaults.AsQueryable(),
                _overrides.AsQueryable(), _lead, WorkTeamCapability.TeamAllocations, Today).Count());

            Override(_lead, WorkTeamPrivilege.ManageMembers, true);
            Assert.AreEqual(1, WorkTeamAccessRules.QualifyingActorRows(_members.AsQueryable(), _defaults.AsQueryable(),
                _overrides.AsQueryable(), _lead, WorkTeamCapability.TeamAllocations, Today).Count());
            CollectionAssert.Contains(Reach(_lead, WorkTeamCapability.ManageTeam, Today, Today), _member);
            CollectionAssert.DoesNotContain(Reach(_lead, WorkTeamCapability.ManageTeam, Today, Today), _manager);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void CaptureTime_DefaultOn_OverrideOffRemovesTheMembership()
        {
            var capturing = WorkTeamAccessRules.WithPrivilege(_members.AsQueryable(), _defaults.AsQueryable(),
                _overrides.AsQueryable(), WorkTeamPrivilege.CaptureTime).Select(m => m.UserAccountId).ToList();
            CollectionAssert.Contains(capturing, _member);

            Override(_member, WorkTeamPrivilege.CaptureTime, false);
            capturing = WorkTeamAccessRules.WithPrivilege(_members.AsQueryable(), _defaults.AsQueryable(),
                _overrides.AsQueryable(), WorkTeamPrivilege.CaptureTime).Select(m => m.UserAccountId).ToList();
            CollectionAssert.DoesNotContain(capturing, _member);
            CollectionAssert.Contains(capturing, _lead);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void TargetPeriodMustCoverRecordDate()
        {
            Override(_manager, WorkTeamPrivilege.TeamTimesheets, true);

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
            Override(_manager, WorkTeamPrivilege.TeamTimesheets, true);
            _members.Single(m => m.UserAccountId == _manager).EndDate = Today.AddDays(-1);

            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Timesheets, Today.AddMonths(-1), Today.AddMonths(-1)).Count);
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void InactiveTeam_GrantsNothing()
        {
            Override(_manager, WorkTeamPrivilege.TeamTimesheets, true);
            _team.IsActive = false;

            Assert.AreEqual(0, Reach(_manager, WorkTeamCapability.Timesheets, Today, Today).Count);
            Assert.AreEqual(0, WorkTeamAccessRules.OwnRowsOverlapping(_members.AsQueryable(), _member, Today, Today).Count());
        }

        [TestMethod]
        [TestCategory("Provider.WorkTeam")]
        public void MultipleTeams_ReachOnlyThroughQualifyingTeam()
        {
            var other = new WorkTeam { Name = "Beta", IsActive = true };
            _members.Add(Row(other, _manager, WorkTeamRoleType.Manager, Today.AddYears(-1), null));
            _members.Add(Row(other, _outsider, WorkTeamRoleType.Member, Today.AddYears(-1), null));
            Override(_manager, WorkTeamPrivilege.TeamTimesheets, true);

            var reach = Reach(_manager, WorkTeamCapability.Timesheets, Today, Today);
            CollectionAssert.Contains(reach, _member);
            CollectionAssert.DoesNotContain(reach, _outsider);

            var teams = WorkTeamAccessRules.ReachingTeamIds(_members.AsQueryable(), _defaults.AsQueryable(), _overrides.AsQueryable(),
                _manager, _member, WorkTeamCapability.Rates, Today, Today, Today).ToList();
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
