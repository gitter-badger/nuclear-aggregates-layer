﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class BargainFileObtainer : IBusinessModelEntityObtainer<BargainFile>, IAggregateReadModel<Bargain>
    {
        private readonly IFinder _finder;

        public BargainFileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BargainFile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BargainFileDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<BargainFile>(dto.Id)).One()
                ?? new BargainFile { IsActive = true };
            
            entity.FileKind = dto.FileKind;
            entity.FileId = dto.FileId;
            entity.BargainId = dto.BargainRef.Id.Value;
            entity.Comment = dto.Comment;
            entity.Timestamp = dto.Timestamp;
            
            return entity;
        }
    }
}