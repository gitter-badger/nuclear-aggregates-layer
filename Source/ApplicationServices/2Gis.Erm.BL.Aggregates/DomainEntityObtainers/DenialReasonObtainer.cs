﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.Aggregates.DomainEntityObtainers
{
    public sealed class DenialReasonObtainer : ISimplifiedModelEntityObtainer<DenialReason>
    {
        private readonly IFinder _finder;

        public DenialReasonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public DenialReason ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (DenialReasonDomainEntityDto)domainEntityDto;
            var entity = _finder.FindOne(Specs.Find.ById<DenialReason>(dto.Id))
                         ?? new DenialReason { IsActive = true };

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.ProofLink = dto.ProofLink;
            entity.Type = (int)dto.Type;
            entity.Timestamp = dto.Timestamp;
            return entity;
        }
    }
}