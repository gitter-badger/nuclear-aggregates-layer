﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListFirmService : ListEntityDtoServiceBase<Firm, ListFirmDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListFirmService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListFirmDto> GetListData(IQueryable<Firm> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var createdInCurrentMonthFilter = filterManager.CreateForExtendedProperty<Firm, bool>(
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

            var organizationUnitFilter = filterManager.CreateForExtendedProperty<Firm, long>(
                "organizationUnitId", organizationUnitId => x => x.OrganizationUnitId == organizationUnitId);

            var clientFilter = filterManager.CreateForExtendedProperty<Firm, long>(
                "clientId", clientId => x => x.ClientId == clientId);

            return query
                .Where(x => !x.IsDeleted)
                .ApplyFilter(clientFilter)
                .ApplyFilter(createdInCurrentMonthFilter)
                .ApplyFilter(organizationUnitFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        x.Name,

                        ClientId = x.Client != null ? x.Client.Id : (long?)null,
                        ClientName = x.Client != null ? x.Client.Name : null,

                        x.OwnerCode,

                        TerritoryId = x.Territory != null ? x.Territory.Id : (long?)null,
                        TerritoryName = x.Territory != null ? x.Territory.Name : null,

                        x.IsActive,
                        x.PromisingScore,
                        x.ClosedForAscertainment,
                        x.LastQualifyTime,
                        x.LastDisqualifyTime,
                        OrganizationUnitName = x.OrganizationUnit.Name
                    })
                .AsEnumerable()
                .Select(x =>
                        new ListFirmDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                ClientId = x.ClientId,
                                ClientName = x.ClientName,
                                OwnerCode = x.OwnerCode,
                                OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                                TerritoryId = x.TerritoryId,
                                TerritoryName = x.TerritoryName,
                                IsActive = x.IsActive,
                                PromisingScore = x.PromisingScore,
                                ClosedForAscertainment = x.ClosedForAscertainment,
                                LastQualifyTime = x.LastQualifyTime,
                                LastDisqualifyTime = x.LastDisqualifyTime,
                                OrganizationUnitName = x.OrganizationUnitName
                            });
        }
    }
}