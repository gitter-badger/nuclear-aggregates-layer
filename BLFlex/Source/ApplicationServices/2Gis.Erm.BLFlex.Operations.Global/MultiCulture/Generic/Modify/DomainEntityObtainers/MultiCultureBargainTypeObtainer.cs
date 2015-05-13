using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
    public sealed class MultiCultureBargainTypeObtainer : ISimplifiedModelEntityObtainer<BargainType>, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                                          IUkraineAdapted
    {
        private readonly IFinder _finder;

        public MultiCultureBargainTypeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BargainType ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (MultiCultureBargainTypeDomainEntityDto)domainEntityDto;

            var entity = _finder.FindOne(Specs.Find.ById<BargainType>(dto.Id)) ??
                         new BargainType { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            BargainTypeFlexSpecs.BargainTypes.MultiCulture.Assign.Entity().Assign(dto, entity);

            return entity;
        }
    }
}