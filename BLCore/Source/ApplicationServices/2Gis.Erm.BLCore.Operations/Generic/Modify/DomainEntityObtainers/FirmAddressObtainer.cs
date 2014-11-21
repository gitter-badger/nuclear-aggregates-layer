﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class FirmAddressObtainer : IBusinessModelEntityObtainer<FirmAddress>, IAggregateReadModel<Firm>
    {
        private readonly IFinder _finder;

        public FirmAddressObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public FirmAddress ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (FirmAddressDomainEntityDto)domainEntityDto;

            var entity = _finder.FindOne(Specs.Find.ById<FirmAddress>(dto.Id)) 
                ?? new FirmAddress { IsActive = true };

            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}