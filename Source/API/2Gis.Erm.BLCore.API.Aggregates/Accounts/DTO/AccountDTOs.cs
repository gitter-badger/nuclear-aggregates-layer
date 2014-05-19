using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OperationTypeDto
    {
        public OperationType OperationType { get; set; }
        public bool AllAccountDetailsIsDeleted { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class LimitDto
    {
        public long LegalPersonId { get; set; }
        public string LegalPersonName { get; set; }
        public long BranchOfficeOrganizationUnitId { get; set; }
        public long BranchOfficeId { get; set; }
        public string BranchOfficeName { get; set; }
        public long LegalPersonOwnerId { get; set; }
        public decimal Amount { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class GetLockDetailDto
    {
        public LockDetail LockDetail { get; set; }
        public Lock Lock { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class AccountFor1CExportDto
    {
        public string LegalPersonSyncCode1C { get; set; }
        public string BranchOfficeOrganizationUnitSyncCode1C { get; set; }
    }
}