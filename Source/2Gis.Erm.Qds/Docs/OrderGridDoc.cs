using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.Docs
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
        public byte HasDocumentsDebt { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public double PayablePlan { get; set; }
        public string WorkflowStep { get; set; }

        public double AmountToWithdraw { get; set; }
        public double AmountWithdrawn { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}