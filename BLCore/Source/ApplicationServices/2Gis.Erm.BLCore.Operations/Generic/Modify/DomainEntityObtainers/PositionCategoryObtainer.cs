using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class PositionCategoryObtainer : IBusinessModelEntityObtainer<PositionCategory>, IAggregateReadModel<Position>
    {
        private readonly IFinder _finder;

        public PositionCategoryObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public PositionCategory ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PositionCategoryDomainEntityDto)domainEntityDto;

            var positionCategory = _finder.Find(Specs.Find.ById<PositionCategory>(dto.Id)).SingleOrDefault() ??
                                   new PositionCategory { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && positionCategory.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            positionCategory.Name = dto.Name;
            positionCategory.ExportCode = dto.ExportCode;
            positionCategory.IsSupportedByExport = dto.IsSupportedByExport;
            positionCategory.Timestamp = dto.Timestamp;

            return positionCategory;
        }
    }
}