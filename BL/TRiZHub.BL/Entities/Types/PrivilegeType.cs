#region Usings

using System;

#endregion

namespace TRiZHub.BL.Entities.Types
{
    [Serializable]
    public enum PrivilegeType
    {
        RoleMaintenance,
        ClientMaintenance,
        ProjectMaintenance,
        ActivityMaintenance,
        ReportGenerationTimesheet,
        ReportGenerationUserSummary,
        ReportGenerationScoreCard,
        BillingCycleMaintenance,
        TeamMaintenance,
        TimesheetCapture,
        TimesheetCaptureForOtherAccounts,
        UserMaintenance,
        UserEmergencyContactMaintenance,
        UserTravelInformationMaintenance,
        UserBillingRatesMaintenance,
        UserAssetRegisterMaintenance,
        UserPersonalInformationMaintenance,
        UserTeamJobDesignationMaintenance,
        ScorecardTemplateMaintenance,
        PerformanceManagementCreateScoreCards,
        PerformanceManagementViewMyScoreCards,
        PerformanceManagementViewMyTeamScoreCards,
        CustomerReportAccess,
        EmployerMaintenance,
        PerformanceManagementAdmin,
        ReportGenerationUserProjects,
        ReportGenerationUserRoles,
        UserProjectMaintenance
    }
}


