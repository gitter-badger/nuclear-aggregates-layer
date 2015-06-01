using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify.DomainEntityObtainers
{
    public sealed class EmiratesBargainTypeObtainer : ISimplifiedModelEntityObtainer<BargainType>, IEmiratesAdapted
    {
        private readonly IFinder _finder;

        public EmiratesBargainTypeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BargainType ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (EmiratesBargainTypeDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<BargainType>(dto.Id)).One() ??
                         new BargainType { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            BargainTypeFlexSpecs.BargainTypes.Emirates.Assign.Entity().Assign(dto, entity);

            return entity;
        }
    }
}