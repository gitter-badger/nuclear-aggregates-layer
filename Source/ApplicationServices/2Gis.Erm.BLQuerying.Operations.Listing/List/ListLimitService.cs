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
            IFinder finder,
            IUserContext userContext,
            ISecurityServiceUserIdentifier userIdentifierService,
            FilterHelper filterHelper)
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

            var myBranchFilter = querySettings.CreateForExtendedProperty<Limit, bool>("MyBranch", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Account.LegalPerson.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var myFilter = querySettings.CreateForExtendedProperty<Limit, bool>("ForMe", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            var myInspectionFilter = querySettings.CreateForExtendedProperty<Limit, bool>("MyInspection", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.InspectorCode == userId;
            });

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
                .Filter(_filterHelper, nextMonthForStartPeriodDateFilter, myBranchFilter, myFilter, myInspectionFilter)
                .Select(x => new ListLimitDto
                {
                    Id = x.Id,
                    BranchOfficeId = x.Account.BranchOfficeOrganizationUnit.BranchOfficeId,
                    BranchOfficeName = x.Account.BranchOfficeOrganizationUnit.BranchOffice.Name,
                    LegalPersonName = x.Account.LegalPerson.LegalName,
                    CreatedOn = x.CreatedOn,
                    CloseDate = x.CloseDate,
                    Amount = x.Amount,
                    ClientId = x.Account.LegalPerson.ClientId,
                    ClientName = x.Account.LegalPerson.Client.Name,
                    OwnerCode = x.OwnerCode,
                    InspectorCode = x.InspectorCode,
                    StatusEnum = (LimitStatus)x.Status,
                    AccountId = x.AccountId,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    LegalPersonId = x.Account.LegalPersonId,
                    OwnerName = null,
                    Status = null,
                    InspectorName = null,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.Status = x.StatusEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                    x.InspectorName = _userIdentifierService.GetUserInfo(x.InspectorCode).DisplayName;

                    return x;
                });
        }
    }
}