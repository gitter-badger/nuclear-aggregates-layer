using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAccountService : ListEntityDtoServiceBase<Account, ListAccountDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListAccountService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext, 
            ISecurityServiceUserIdentifier userIdentifierService)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListAccountDto> GetListData(
            IQueryable<Account> query,
            QuerySettings querySettings,
            ListFilterManager filterManager,
            out int count)
        {
            var withHostedOrdersFilter = filterManager.CreateForExtendedProperty<Account, bool>(
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
                .ApplyFilter(withHostedOrdersFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new 
                    {
                        Id = x.Id,
                        IsDeleted = x.IsDeleted,
                        CurrencyId = x.BranchOfficeOrganizationUnit.OrganizationUnit.Country.CurrencyId,
                        LegalPersonId = x.LegalPersonId,
                        ClientId = x.LegalPerson.ClientId,
                        ClientName = x.LegalPerson.Client.Name,
                        Inn = x.LegalPerson.Inn,
                        BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                        BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnit.ShortLegalName,
                        LegalPersonName = x.LegalPerson.LegalName,
                        CurrencyName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Country.Currency.Name,
                        OrganizationUnitId = x.BranchOfficeOrganizationUnit.OrganizationUnit.Id,
                        OrganizationUnitName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Name,
                        AccountDetailBalance = x.Balance,
                        CreateDate = x.CreatedOn,
                        OwnerCode = x.OwnerCode,
                    }).AsEnumerable()
                .Select(x => new ListAccountDto
                    {
                        Id = x.Id,
                        IsDeleted = x.IsDeleted,
                        CurrencyId = x.CurrencyId,
                        LegalPersonId = x.LegalPersonId,
                        ClientId = x.ClientId,
                        ClientName = x.ClientName,
                        Inn = x.Inn,
                        BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                        BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnitName,
                        LegalPersonName = x.LegalPersonName,
                        CurrencyName = x.CurrencyName,
                        OrganizationUnitId = x.OrganizationUnitId,
                        OrganizationUnitName = x.OrganizationUnitName,
                        AccountDetailBalance = x.AccountDetailBalance,
                        CreateDate = x.CreateDate,
                        OwnerCode = x.OwnerCode,
                        OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName
                    }).ToArray();
        }
    }
}
