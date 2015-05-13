using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListLocalMessageService : ListEntityDtoServiceBase<LocalMessage, ListLocalMessageDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListLocalMessageService(
            IQuery query,
            FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<LocalMessage>();

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
                    StatusEnum = x.Status,
                    SenderSystemEnum = (IntegrationSystem)x.MessageType.SenderSystem,
                    ReceiverSystemEnum = (IntegrationSystem)x.MessageType.ReceiverSystem,
                    IntegrationType = null,
                    Status = x.Status.ToStringLocalizedExpression(),
                    ReceiverSystem = ((IntegrationSystem)x.MessageType.ReceiverSystem).ToStringLocalizedExpression(),
                    SenderSystem = ((IntegrationSystem)x.MessageType.SenderSystem).ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListLocalMessageDto dto)
        {
            dto.IntegrationType = dto.IntegrationTypeImport ?? dto.IntegrationTypeExport;
        }
    }
}