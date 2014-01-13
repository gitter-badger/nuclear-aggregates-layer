﻿using System.Collections.Generic;
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
    public class ListOperationService : ListEntityDtoServiceBase<Operation, ListOperationDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListOperationService(IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider, 
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext) 
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListOperationDto> GetListData(IQueryable<Operation> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                            {
                                x.Id,
                                x.StartTime,
                                x.FinishTime,
                                x.Type,
                                x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnit.Name,
                                x.Status,
                                x.OwnerCode,
                                x.Description
                            })
                .AsEnumerable()
                .Select(x =>
                        new ListOperationDto
                            {
                                Id = x.Id,
                                StartTime = x.StartTime,
                                FinishTime = x.FinishTime,
                                Type = ((BusinessOperation)x.Type).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OrganizationUnitId = x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnitName,
                                Status = ((OperationStatus)x.Status).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OwnerCode = x.OwnerCode,
                                Owner = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                                Description = x.Description
                            });
        }
    }
}
