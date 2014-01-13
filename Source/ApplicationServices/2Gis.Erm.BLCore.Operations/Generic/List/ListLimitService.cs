using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListLimitService : ListEntityDtoServiceBase<Limit, ListLimitDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListLimitService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext,
            ISecurityServiceUserIdentifier userIdentifierService)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListLimitDto> GetListData(IQueryable<Limit> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var nextMonthForStartPeriodDateFilter = filterManager.CreateForExtendedProperty<Limit, bool>(
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
                .ApplyFilter(nextMonthForStartPeriodDateFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        BranchOfficeName = x.Account.BranchOfficeOrganizationUnit.BranchOffice.Name,
                        LegalPersonName = x.Account.LegalPerson.LegalName,
                        x.CreatedOn,
                        x.CloseDate,
                        x.Amount,
                        ClientName = x.Account.LegalPerson.Client.Name,
                        x.OwnerCode,
                        x.InspectorCode,
                        Status = (LimitStatus)x.Status
                    })
                .AsEnumerable()
                .Select(x => new ListLimitDto
                    {
                        Id = x.Id,
                        BranchOfficeName = x.BranchOfficeName,
                        LegalPersonName = x.LegalPersonName,
                        CreatedOn = x.CreatedOn,
                        CloseDate = x.CloseDate,
                        Amount = x.Amount,
                        Status = x.Status.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                        ClientName = x.ClientName,
                        OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                        InspectorName = _userIdentifierService.GetUserInfo(x.InspectorCode).DisplayName,
                    });
        }
    }
}