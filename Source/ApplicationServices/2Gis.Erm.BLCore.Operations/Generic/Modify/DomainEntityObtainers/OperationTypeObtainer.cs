using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class OperationTypeObtainer : IBusinessModelEntityObtainer<OperationType>, IAggregateReadModel<Account>
    {
        private readonly IFinder _finder;

        public OperationTypeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public OperationType ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (OperationTypeDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(OperationSpecifications.Find.OperationTypeById(dto.Id)).SingleOrDefault() ??
                         new OperationType { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsPlus = dto.IsPlus;
            entity.IsInSyncWith1C = dto.IsInSyncWith1C;
            entity.SyncCode1C = dto.SyncCode1C;
            return entity;
        }
    }
}