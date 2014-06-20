﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AssociatedPositionObtainer : IBusinessModelEntityObtainer<AssociatedPosition>, IAggregateReadModel<Price>
    {
        private readonly IFinder _finder;

        public AssociatedPositionObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AssociatedPosition ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AssociatedPositionDomainEntityDto)domainEntityDto;

            var associatedPosition = _finder.FindOne(Specs.Find.ById<AssociatedPosition>(dto.Id)) 
                ?? new AssociatedPosition { IsActive = true };

            associatedPosition.PositionId = dto.PositionRef.Id.Value;
            associatedPosition.AssociatedPositionsGroupId = dto.AssociatedPositionsGroupRef.Id.Value;
            associatedPosition.ObjectBindingType = (int)dto.ObjectBindingType;
            associatedPosition.Timestamp = dto.Timestamp;

            return associatedPosition;
        }
    }
}