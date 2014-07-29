using System;

using DoubleGis.Erm.BLCore.API.Common.Enums;

namespace DoubleGis.Erm.BLCore.WCF.MoDi.Util
{
    internal class BouInfo
    {
        public long Id { get; set; }
        public string RegistrationCertificate { get; set; }
        public DateTime FirstEmitDate { get; set; }
        public string ChiefNameInGenitive { get; set; }
        public string ChiefNameInNominative { get; set; }
        public string ShortLegalName { get; set; }
        public string PositionInGenitive { get; set; }
        public string OperatesOnTheBasisInGenitive { get; set; }
        public string Email { get; set; }
        public string Kpp { get; set; }
        public string PaymentEssentialElements { get; set; }
        public string SyncCode1C { get; set; }

        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public string OrganizationUnitSyncCode1C { get; set; }
        public string OrganizationUnitElectronicMedia { get; set; }

        public string BranchOfficeInn { get; set; }

        public string BranchOfficeLegalAddress { get; set; }
        public string BranchOfficeBargainTypeSyncCode1C { get; set; }
        public ContributionTypeEnum BranchOfficeContributionType { get; set; }

    }
}