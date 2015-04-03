using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.Operations.Generic.Get
{
    public class GetDenialReasonDtoOperationService : GetDomainEntityDtoServiceBase<DenialReason>
    {
        private readonly IDenialReasonReadModel _readModel;

        public GetDenialReasonDtoOperationService(IUserContext userContext, IDenialReasonReadModel readModel)
            : base(userContext)
        {
            _readModel = readModel;
        }

        protected override IDomainEntityDto<DenialReason> GetDto(long entityId)
        {
            var entity = _readModel.GetDenialReason(entityId);
            return new DenialReasonDomainEntityDto
                       {
                           Id = entity.Id,
                           Name = entity.Name,
                           Description = entity.Description,
                           ProofLink = entity.ProofLink,
                           Type = entity.Type,
                           IsActive = entity.IsActive,
                           CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                           CreatedOn = entity.CreatedOn,
                           ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                           ModifiedOn = entity.ModifiedOn,
                           Timestamp = entity.Timestamp
                       };
        }

        protected override IDomainEntityDto<DenialReason> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new DenialReasonDomainEntityDto();
        }
    }
}