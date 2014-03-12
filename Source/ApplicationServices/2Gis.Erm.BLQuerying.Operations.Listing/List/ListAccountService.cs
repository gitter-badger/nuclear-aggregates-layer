using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAccountService : ListEntityDtoServiceBase<Account, ListAccountDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListAccountService(
            IQuerySettingsProvider querySettingsProvider,
            IFinder finder,
            FilterHelper filterHelper, 
            ISecurityServiceUserIdentifier userIdentifierService)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListAccountDto> List(QuerySettings querySettings,
            out int count)
        {
            var query = _finder.FindAll<Account>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

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
                                (y.WorkflowStepId == (int)OrderState.Approved || y.WorkflowStepId == (int)OrderState.OnTermination) &&
                                y.EndDistributionDateFact >= currentMonthLastDate && y.BeginDistributionDate <= currentMonthFirstDate);
                    });

            return query
            .Filter(_filterHelper, withHostedOrdersFilter)
            .DefaultFilter(_filterHelper, querySettings)
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

                OwnerCode = x.OwnerCode,

                OrganizationUnitId = x.BranchOfficeOrganizationUnit.OrganizationUnit.Id,
                OrganizationUnitName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Name,
            })
            .QuerySettings(_filterHelper, querySettings, out count)
            .Select(x =>
            {
                x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                return x;
            });
        }
    }
}
