using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListLimitService : ListEntityDtoServiceBase<Limit, ListLimitDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly FilterHelper _filterHelper;

        public ListLimitService(
            IQuerySettingsProvider querySettingsProvider,
            IFinder finder,
            IUserContext userContext,
            ISecurityServiceUserIdentifier userIdentifierService,
            FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _userContext = userContext;
            _userIdentifierService = userIdentifierService;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListLimitDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Limit>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var nextMonthForStartPeriodDateFilter = querySettings.CreateForExtendedProperty<Limit, bool>(
                "useNextMonthForStartPeriodDate",
                useNextMonth =>
                {
                    if (!useNextMonth)
                    {
                        return null;
                    }

                    var nextMonth = DateTime.Now.AddMonths(1);
                    nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                    return x => x.StartPeriodDate == nextMonth;
                });

            return query
                .Filter(_filterHelper, nextMonthForStartPeriodDateFilter)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                    {
                        x.Id,
                        x.Account.BranchOfficeOrganizationUnit.BranchOfficeId,
                        BranchOfficeName = x.Account.BranchOfficeOrganizationUnit.BranchOffice.Name,
                        LegalPersonName = x.Account.LegalPerson.LegalName,
                        x.CreatedOn,
                        x.CloseDate,
                        x.Amount,
                        x.Account.LegalPerson.ClientId,
                        ClientName = x.Account.LegalPerson.Client.Name,
                        x.OwnerCode,
                        x.InspectorCode,
                        Status = (LimitStatus)x.Status,
                        x.AccountId,
                        x.IsActive,
                        x.IsDeleted,
                        x.Account.LegalPersonId,
                    })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ListLimitDto
                    {
                        Id = x.Id,
                        BranchOfficeName = x.BranchOfficeName,
                        LegalPersonName = x.LegalPersonName,
                        CreatedOn = x.CreatedOn,
                        CloseDate = x.CloseDate,
                        Amount = x.Amount,
                        Status = x.Status.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                        ClientName = x.ClientName,
                        OwnerCode = x.OwnerCode,
                        OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                        InspectorCode = x.InspectorCode,
                        InspectorName = _userIdentifierService.GetUserInfo(x.InspectorCode).DisplayName,
                        AccountId = x.AccountId,
                        ClientId = x.ClientId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        LegalPersonId = x.LegalPersonId,
                        BranchOfficeId = x.BranchOfficeId,
                    });
        }
    }
}