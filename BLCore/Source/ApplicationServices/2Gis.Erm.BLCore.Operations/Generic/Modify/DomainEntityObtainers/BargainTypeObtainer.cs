using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class BargainTypeObtainer : ISimplifiedModelEntityObtainer<BargainType>
    {
        private readonly IFinder _finder;

        public BargainTypeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BargainType ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BargainTypeDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<BargainType>(dto.Id)).SingleOrDefault() ??
                         new BargainType { IsActive = true, Id = dto.Id  };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.Name = dto.Name;
            entity.SyncCode1C = dto.SyncCode1C;
            entity.VatRate = dto.VatRate;
            entity.Timestamp = dto.Timestamp;
            
            return entity;
        }
    }
}