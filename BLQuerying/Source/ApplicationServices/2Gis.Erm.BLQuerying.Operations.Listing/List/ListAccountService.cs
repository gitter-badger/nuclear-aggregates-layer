using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAccountService : ListEntityDtoServiceBase<Account, ListAccountDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IUserContext _userContext;

        public ListAccountService(
            IQuery query,
            FilterHelper filterHelper, 
            ISecurityServiceUserIdentifier userIdentifierService,
            IUserContext userContext)
        {
            _query = query;
            _filterHelper = filterHelper;
            _userIdentifierService = userIdentifierService;
            _userContext = userContext;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Account>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var myBranchFilter = querySettings.CreateForExtendedProperty<Account, bool>("MyBranch", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.LegalPerson.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var withHostedOrdersFilter = querySettings.CreateForExtendedProperty<Account, bool>(
                "WithHostedOrders",
                withHostedOrders =>
                    {
                        if (!withHostedOrders)
                        {
                            return null;
                        }

                        var nextMonth = DateTime.Now.AddMonths(1);
                        nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                        var currentMonthLastDate = nextMonth.AddSeconds(-1);
                        var currentMonthFirstDate = new DateTime(currentMonthLastDate.Year, currentMonthLastDate.Month, 1);

                        return
                            x =>
                            x.Orders.Any(
                                y =>
                                y.IsActive && !y.IsDeleted &&
                                (y.WorkflowStepId == OrderState.Approved || y.WorkflowStepId == OrderState.OnTermination) &&
                                y.EndDistributionDateFact >= currentMonthLastDate && y.BeginDistributionDate <= currentMonthFirstDate);
                    });

            return query
            .Filter(_filterHelper, withHostedOrdersFilter, myBranchFilter)
            .Select(x => new ListAccountDto
            {
                Id = x.Id,

                BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnit.ShortLegalName,

                LegalPersonId = x.LegalPersonId,
                LegalPersonName = x.LegalPerson.LegalName,

                ClientId = x.LegalPerson.ClientId,
                ClientName = x.LegalPerson.Client.Name,

                Inn = x.LegalPerson.Inn,
                AccountDetailBalance = x.Balance,

                CurrencyId  = x.BranchOfficeOrganizationUnit.OrganizationUnit.Country.CurrencyId,
                CurrencyName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Country.Currency.Name,

                CreateDate = x.CreatedOn,
                IsDeleted = x.IsDeleted,
                IsActive = x.IsActive,

                OwnerCode = x.OwnerCode,
                OwnerName = null,

                OrganizationUnitId = x.BranchOfficeOrganizationUnit.OrganizationUnit.Id,
                OrganizationUnitName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Name,

                Balance = x.Balance,
            })
            .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListAccountDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}
