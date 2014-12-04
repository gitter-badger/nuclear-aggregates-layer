using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface ICustomizableOrderViewModel : IEntityViewModelBase
    {
        Uri OrderValidationServiceUrl { get; set; }
        int WorkflowStepId { get; set; }
        int PreviousWorkflowStepId { get; set; }
        long CurrenctUserCode { get; set; }
        bool CanEditOrderType { get; set; }
        bool HasOrderBranchOfficeOrganizationUnitSelection { get; set; }
        bool HasOrderDocumentsDebtChecking { get; set; }
        LookupField SourceOrganizationUnit { get; set; }
        LookupField DestinationOrganizationUnit { get; set; }
        LookupField Inspector { get; set; }
        bool IsWorkflowLocked { get; set; }
        string AvailableSteps { get; set; }
        DateTime SignupDate { get; set; }
        DateTime BeginDistributionDate { get; set; }
        DateTime EndDistributionDateFact { get; set; }
        bool IsTerminated { get; set; }
    }
}