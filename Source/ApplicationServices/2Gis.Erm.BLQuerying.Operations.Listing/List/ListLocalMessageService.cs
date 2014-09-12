﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListLocalMessageService : ListEntityDtoServiceBase<LocalMessage, ListLocalMessageDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListLocalMessageService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<LocalMessage>();

            return query
                .Select(x => new ListLocalMessageDto
                {
                    Id = x.Id,
                    IntegrationTypeImport = ((IntegrationTypeImport)x.MessageType.IntegrationType).ToStringLocalizedExpression(),
                    IntegrationTypeExport = ((IntegrationTypeExport)x.MessageType.IntegrationType).ToStringLocalizedExpression(),
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    StatusEnum = (LocalMessageStatus)x.Status,
                    SenderSystemEnum = (IntegrationSystem)x.MessageType.SenderSystem,
                    ReceiverSystemEnum = (IntegrationSystem)x.MessageType.ReceiverSystem,
                    IntegrationType = null,
                    Status = ((LocalMessageStatus)x.Status).ToStringLocalizedExpression(),
                    ReceiverSystem = ((IntegrationSystem)x.MessageType.ReceiverSystem).ToStringLocalizedExpression(),
                    SenderSystem = ((IntegrationSystem)x.MessageType.SenderSystem).ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings)
                .Transform(x =>
                {
                    x.IntegrationType = x.IntegrationTypeImport ?? x.IntegrationTypeExport;
                    return x;
                });
        }
    }
}