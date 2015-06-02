using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Specs;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.List
{
    public sealed class ListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, ListLegalPersonDto>, IRussiaAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public ListLegalPersonService(
            ISecurityServiceUserIdentifier userIdentifierService, 
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext,
            IDebtProcessingSettings debtProcessingSettings)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
            _debtProcessingSettings = debtProcessingSettings;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<LegalPerson>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.Deal()) && querySettings.ParentEntityId.HasValue)
            {
                var clientId = _finder.Find(Specs.Find.ById<Deal>(querySettings.ParentEntityId.Value)).Select(x => x.ClientId).Single();
                query = _filterHelper.ForClientAndItsDescendants(query, clientId);
            }

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.Client()) && querySettings.ParentEntityId.HasValue)
            {
                query = _filterHelper.ForClientAndItsDescendants(query, querySettings.ParentEntityId.Value);
            }

            var debtFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("WithDebt", info =>
            {
                var minDebtAmount = _debtProcessingSettings.MinDebtAmount;
                return x => x.Accounts.Any(y => !y.IsDeleted && y.IsActive && y.Balance < minDebtAmount);
            });

            var hasMyOrdersFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("HasMyOrders", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Orders.Any(y => !y.IsDeleted && y.IsActive && y.OwnerCode == userId);
            });

            var myBranchFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("MyBranch", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var restrictForMergeFilter = querySettings.CreateForExtendedProperty<LegalPerson, long>(
                "restrictForMergeId",
                restrictForMergeId =>
                    {
                        var restrictedLegalPerson =
                            query.Where(x => x.Id == restrictForMergeId)
                                 .Select(x => new
                                     {
                                         LegalPersonTypeEnum = x.LegalPersonTypeEnum,
                                         Inn = x.Inn,
                                         Kpp = x.Kpp,
                                         PassportNumber = x.PassportNumber,
                                         PassportSeries = x.PassportSeries
                                     }).SingleOrDefault();
                        if (restrictedLegalPerson != null)
                        {
                            var legalPersonType =
                                restrictedLegalPerson.LegalPersonTypeEnum;
                            switch (legalPersonType)
                            {
                                case LegalPersonType.LegalPerson:
                                    return
                                        x =>
                                        x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                        x.Inn == restrictedLegalPerson.Inn &&
                                        x.Kpp == restrictedLegalPerson.Kpp &&
                                        x.LegalPersonTypeEnum == restrictedLegalPerson.LegalPersonTypeEnum;
                                case LegalPersonType.Businessman:
                                    return
                                        x =>
                                        x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                        x.Inn == restrictedLegalPerson.Inn &&
                                        x.LegalPersonTypeEnum == restrictedLegalPerson.LegalPersonTypeEnum;
                                case LegalPersonType.NaturalPerson:
                                    return
                                        x =>
                                        x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                        x.PassportNumber == restrictedLegalPerson.PassportNumber &&
                                        x.PassportSeries == restrictedLegalPerson.PassportSeries &&
                                        x.LegalPersonTypeEnum == restrictedLegalPerson.LegalPersonTypeEnum;
                            }
                        }

                        return x => x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted;
                    });

            var dealFilter = querySettings.CreateForExtendedProperty<LegalPerson, long>("dealId", dealId => LegalPersonListSpecs.Filter.ByDeal(dealId, _finder));

            return query
                .Filter(_filterHelper, dealFilter, debtFilter, hasMyOrdersFilter, myBranchFilter, restrictForMergeFilter)
                .Select(x => new ListLegalPersonDto
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    ShortName = x.ShortName,
                    Inn = x.Inn,
                    Kpp = x.Kpp,
                    LegalAddress = x.LegalAddress,
                    PassportSeries = x.PassportSeries,
                    PassportNumber = x.PassportNumber,
                    ClientId = x.ClientId,
                    ClientName = x.Client.Name,
                    OwnerCode = x.OwnerCode,
                    CreatedOn = x.CreatedOn,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    LegalPersonTypeEnum = x.LegalPersonTypeEnum,
                    OwnerName = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListLegalPersonDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}