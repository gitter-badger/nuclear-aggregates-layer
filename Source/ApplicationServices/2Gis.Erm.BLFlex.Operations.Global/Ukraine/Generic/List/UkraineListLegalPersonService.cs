using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

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

        protected override IEnumerable<UkraineListLegalPersonDto> List(QuerySettings querySettings, out int count)
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

            var myFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("ForMe", forMe =>
            {
                var userId = _userContext.Identity.Code;
                if (forMe)
                {
                    return x => x.OwnerCode == userId;
                }

                return x => x.OwnerCode != userId;
            });

            return query
                .Filter(_filterHelper, debtFilter, hasMyOrdersFilter, myBranchFilter, myFilter)
                // FIXME {y.baranihin, 19.03.2014}: Эта выборка точно нужна? Ниже, в join снова выбираются поля в анонимный объект.
                // DONE {a.rechkalov, 20.03.2014}: убрал
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
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new UkraineListLegalPersonDto
                    {
                        Id = x.Id,
                        LegalName = x.LegalName,
                        LegalAddress = x.LegalAddress,
                        ClientId = x.ClientId,
                        ClientName = x.ClientName,
                        OwnerCode = x.OwnerCode,
                        OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                        Ipn = x.Ipn,
                        Egrpou = x.Egrpou,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted
                    });
        }
    }
}