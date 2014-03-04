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
    public sealed class ListOperationService : ListEntityDtoServiceBase<Operation, ListOperationDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListOperationService(IQuerySettingsProvider querySettingsProvider, 
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListOperationDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Operation>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
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
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                        new ListOperationDto
                            {
                                Id = x.Id,
                                StartTime = x.StartTime,
                                FinishTime = x.FinishTime,
                                Type = ((BusinessOperation)x.Type).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OrganizationUnitId = x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnitName,
                                Status = ((OperationStatus)x.Status).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OwnerCode = x.OwnerCode,
                                Owner = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                                Description = x.Description
                            });
        }
    }
}
