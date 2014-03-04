using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmService : ListEntityDtoServiceBase<Firm, ListFirmDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListFirmService(
            IQuerySettingsProvider querySettingsProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListFirmDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Firm>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var createdInCurrentMonthFilter = querySettings.CreateForExtendedProperty<Firm, bool>(
                "CreatedInCurrentMonth",
                createdInCurrentMonth =>
                    {
                        if (!createdInCurrentMonth)
                        {
                            return null;
                        }

                        var nextMonth = DateTime.Now.AddMonths(1);
                        nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                        var currentMonthLastDate = nextMonth.AddSeconds(-1);
                        var currentMonthFirstDate = new DateTime(currentMonthLastDate.Year, currentMonthLastDate.Month, 1);

                        return x => x.CreatedOn >= currentMonthFirstDate && x.CreatedOn <= currentMonthLastDate;
                    });

            var organizationUnitFilter = querySettings.CreateForExtendedProperty<Firm, long>(
                "organizationUnitId", organizationUnitId => x => x.OrganizationUnitId == organizationUnitId);

            var clientFilter = querySettings.CreateForExtendedProperty<Firm, long>(
                "clientId", clientId => x => x.ClientId == clientId);

            return query
                .Where(x => !x.IsDeleted)
                .Filter(_filterHelper, clientFilter, createdInCurrentMonthFilter, organizationUnitFilter)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListFirmDto
                    {
                        Id = x.Id,
                        Name = x.Name,

                        ClientId = x.Client != null ? x.Client.Id : (long?)null,
                        ClientName = x.Client != null ? x.Client.Name : null,

                        OwnerCode = x.OwnerCode,

                        TerritoryId = x.Territory != null ? x.Territory.Id : (long?)null,
                        TerritoryName = x.Territory != null ? x.Territory.Name : null,

                        IsActive = x.IsActive,
                        PromisingScore = x.PromisingScore,
                        ClosedForAscertainment = x.ClosedForAscertainment,
                        LastQualifyTime = x.LastQualifyTime,
                        LastDisqualifyTime = x.LastDisqualifyTime,
                        OrganizationUnitId = x.OrganizationUnit.Id,
                        OrganizationUnitName = x.OrganizationUnit.Name,
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