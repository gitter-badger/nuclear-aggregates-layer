﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class DeniedPositionObtainer : IBusinessModelEntityObtainer<DeniedPosition>, IAggregateReadModel<Price>
    {
        private readonly IFinder _finder;

        public DeniedPositionObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public DeniedPosition ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (DeniedPositionDomainEntityDto)domainEntityDto;

            var deniedPosition =
                dto.Id == 0
                    ? new DeniedPosition { IsActive = true }
                    : _finder.Find(DeniedPositionSpecifications.Find.ById(dto.Id)).Single();

            deniedPosition.PositionId = dto.PositionRef.Id.Value;
            deniedPosition.PositionDeniedId = dto.PositionDeniedRef.Id.Value;
            deniedPosition.PriceId = dto.PriceRef.Id.Value;
            deniedPosition.ObjectBindingType = (int)dto.ObjectBindingType;
            deniedPosition.Timestamp = dto.Timestamp;

            return deniedPosition;
        }
    }
}