﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderProcessingRequestService : ListEntityDtoServiceBase<OrderProcessingRequest, ListOrderProcessingRequestDto>
    {
        private readonly IFinder _finder;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly FilterHelper _filterHelper;

        public ListOrderProcessingRequestService(
            IFinder finder, 
            ISecurityServiceUserIdentifier userIdentifierService, FilterHelper filterHelper)
        {
            _finder = finder;
            _userIdentifierService = userIdentifierService;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
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
                    State = (x.State).ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListOrderProcessingRequestDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}
