using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.WCF.MoDi.Util
{
    internal static class MoDiSpecifications
    {
        public static FindSpecification<BranchOfficeOrganizationUnit> FindAllPrimatyRegionalBranchOffices
        {
            get
            {
                return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsActive && !x.IsDeleted && x.IsPrimaryForRegionalSales);
            }
        }

        public static SelectSpecification<BranchOfficeOrganizationUnit, BouInfo> SelectBouInfo
        {
            get
            {
                return new SelectSpecification<BranchOfficeOrganizationUnit, BouInfo>(x => new BouInfo
                {
                    Id = x.Id,
                    RegistrationCertificate = x.RegistrationCertificate,
                    FirstEmitDate = x.OrganizationUnit.FirstEmitDate,
                    ShortLegalName = x.ShortLegalName,
                    PositionInGenitive = x.PositionInGenitive,
                    ChiefNameInGenitive = x.ChiefNameInGenitive,
                    ChiefNameInNominative = x.ChiefNameInNominative,
                    OperatesOnTheBasisInGenitive = x.OperatesOnTheBasisInGenitive,
                    Email = x.Email,
                    SyncCode1C = x.SyncCode1C,
                    Kpp = x.Kpp,

                    PaymentEssentialElements = x.PaymentEssentialElements,

                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    OrganizationUnitSyncCode1C = x.OrganizationUnit.SyncCode1C,
                    OrganizationUnitElectronicMedia = x.OrganizationUnit.ElectronicMedia,

                    BranchOfficeInn = x.BranchOffice.Inn,
                    BranchOfficeBargainTypeSyncCode1C = x.BranchOffice.BargainType.SyncCode1C,
                    BranchOfficeLegalAddress = x.BranchOffice.LegalAddress,
                    BranchOfficeContributionType = (ContributionTypeEnum)x.BranchOffice.ContributionTypeId.Value,
                });
            }
        }
    }
}