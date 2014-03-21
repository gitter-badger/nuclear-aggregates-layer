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
    public sealed class ListOrderProcessingRequestService : ListEntityDtoServiceBase<OrderProcessingRequest, ListOrderProcessingRequestDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly FilterHelper _filterHelper;

        public ListOrderProcessingRequestService(
            IFinder finder, 
            IUserContext userContext,
            ISecurityServiceUserIdentifier userIdentifierService, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _userIdentifierService = userIdentifierService;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListOrderProcessingRequestDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<OrderProcessingRequest>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListOrderProcessingRequestDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    BaseOrderId = x.BaseOrderId,
                    BaseOrderNumber = x.BaseOrder.Number,
                    RenewedOrderId = x.RenewedOrderId,
                    RenewedOrderNumber = x.RenewedOrder.Number,
                    OwnerCode = x.OwnerCode,
                    StateEnum = (OrderProcessingRequestState)x.State,
                    BeginDistributionDate = x.BeginDistributionDate,
                    FirmId = x.FirmId,
                    FirmName = x.Firm.Name,
                    DueDate = x.DueDate,
                    LegalPersonProfileId = x.LegalPersonProfileId,
                    LegalPersonProfileName = x.LegalPersonProfile.Name,
                    SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                    SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                    CreatedOn = x.CreatedOn,
                    IsDeleted = x.IsDeleted,
                    OwnerName = null,
                    State = null,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                    x.State = x.StateEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);

                    return x;
                });
        }
    }
}
