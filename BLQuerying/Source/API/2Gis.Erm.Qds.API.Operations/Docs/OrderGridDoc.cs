using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class OrderGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Number { get; set; }

        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public DateTime EndDistributionDateFact { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DocumentsDebt HasDocumentsDebt { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public double PayablePlan { get; set; }
        public string WorkflowStep { get; set; }

        public double AmountToWithdraw { get; set; }
        public double AmountWithdrawn { get; set; }

        // relations
        public string FirmId { get; set; }
        public string FirmName { get; set; }
        public string OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public string SourceOrganizationUnitId { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public string DestOrganizationUnitId { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public string LegalPersonId { get; set; }
        public string LegalPersonName { get; set; }
        public string BargainId { get; set; }
        public string BargainNumber { get; set; }
        public string AccountId { get; set; }
        public string ClientId { get; set; }
        public string DealId { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}