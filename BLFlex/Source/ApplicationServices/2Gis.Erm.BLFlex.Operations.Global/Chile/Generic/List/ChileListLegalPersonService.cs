using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Specs;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Security.API.UserContext;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.List
{
    public sealed class ChileListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, ChileListLegalPersonDto>, IChileAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IQuery _query;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public ChileListLegalPersonService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IQuery query,
            IFinder finder,
            FilterHelper filterHelper,
            IDebtProcessingSettings debtProcessingSettings,
            IUserContext userContext)
        {
            _userIdentifierService = userIdentifierService;
            _query = query;
            _finder = finder;
            _filterHelper = filterHelper;
            _debtProcessingSettings = debtProcessingSettings;
            _userContext = userContext;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<LegalPerson>();

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

            var debtFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("WithDebt",
                                                                                        info => LegalPersonListSpecs.Filter.WithDebt(_debtProcessingSettings.MinDebtAmount));

            var hasMyOrdersFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("HasMyOrders",
                                                                                               info => LegalPersonListSpecs.Filter.WithUserOrders(_userContext.Identity.Code));
            

            var myBranchFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("MyBranch",
                                                                                            info => LegalPersonListSpecs.Filter.ByMyBranch(_userContext.Identity.Code));

            var dealFilter = querySettings.CreateForExtendedProperty<LegalPerson, long>("dealId", dealId => LegalPersonListSpecs.Filter.ByDeal(dealId, _finder));
                    
            return query
                .Filter(_filterHelper, dealFilter, debtFilter, hasMyOrdersFilter, myBranchFilter)
                .Select(x => new ChileListLegalPersonDto
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    LegalAddress = x.LegalAddress,
                    ClientId = x.ClientId,
                    ClientName = x.Client.Name,
                    OwnerCode = x.OwnerCode,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Rut = x.Inn,
                    OwnerName = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ChileListLegalPersonDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}