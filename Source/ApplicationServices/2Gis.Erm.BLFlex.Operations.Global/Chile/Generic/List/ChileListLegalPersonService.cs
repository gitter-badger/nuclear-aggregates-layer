using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.List
{
    public sealed class ChileListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, ChileListLegalPersonDto>, IChileAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public ChileListLegalPersonService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper,
            IDebtProcessingSettings debtProcessingSettings,
            IUserContext userContext)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
            _debtProcessingSettings = debtProcessingSettings;
            _userContext = userContext;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<LegalPerson>();

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

            return query
                .Filter(_filterHelper, debtFilter, hasMyOrdersFilter, myBranchFilter)
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