using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Specs;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.List
{
    public class UkraineListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, UkraineListLegalPersonDto>, IUkraineAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public UkraineListLegalPersonService(
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

            var dynamicObjectsQuery = _finder.FindAll<BusinessEntityInstance>().Select(x => new
                {
                    Instance = x,
                    x.BusinessEntityPropertyInstances
                });

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

            var dealFilter = querySettings.CreateForExtendedProperty<LegalPerson, long>("dealId", dealId => LegalPersonListSpecs.Filter.ByDeal(dealId, _finder));

            return query
                .Filter(_filterHelper, dealFilter, debtFilter, hasMyOrdersFilter, myBranchFilter)
                .Join(dynamicObjectsQuery,
                      x => x.Id,
                      y => y.Instance.EntityId,
                      (x, y) =>
                      new UkraineListLegalPersonDto
                      {
                          Id = x.Id,
                          LegalName = x.LegalName,
                          LegalAddress = x.LegalAddress,
                          ClientId = x.ClientId,
                          ClientName = x.Client.Name,
                          OwnerCode = x.OwnerCode,
                          Ipn = x.Inn,
                          Egrpou = y.BusinessEntityPropertyInstances.FirstOrDefault(z => z.PropertyId == EgrpouIdentity.Instance.Id).TextValue,
                          IsActive = x.IsActive,
                          IsDeleted = x.IsDeleted
                      })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(UkraineListLegalPersonDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}