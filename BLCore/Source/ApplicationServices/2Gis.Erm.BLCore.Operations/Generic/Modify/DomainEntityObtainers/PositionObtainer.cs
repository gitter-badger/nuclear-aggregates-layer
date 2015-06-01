using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class PositionObtainer : IBusinessModelEntityObtainer<Position>, IAggregateReadModel<Position>
    {
        private readonly IFinder _finder;

        public PositionObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Position ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PositionDomainEntityDto)domainEntityDto;

            var position = _finder.Find(Specs.Find.ById<Position>(dto.Id)).One() ??
                           new Position { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && position.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            position.Name = dto.Name;
            position.IsComposite = dto.IsComposite;
            position.IsControlledByAmount = dto.IsControlledByAmount;
            position.PlatformId = dto.PlatformRef.Id.Value;
            position.CategoryId = dto.CategoryRef.Id.Value;
            position.BindingObjectTypeEnum = dto.BindingObjectTypeEnum;
            position.CalculationMethodEnum = dto.CalculationMethodEnum;
            position.SalesModel = dto.SalesModel;
            position.PositionsGroup = dto.PositionsGroup;
            position.AdvertisementTemplateId = dto.AdvertisementTemplateRef != null ? dto.AdvertisementTemplateRef.Id : null;
            position.ExportCode = dto.ExportCode;
            position.DgppId = dto.DgppId;
            position.Timestamp = dto.Timestamp;
            position.RestrictChildPositionPlatforms = dto.RestrictChildPositionPlatforms;
            return position;
        }
    }
}