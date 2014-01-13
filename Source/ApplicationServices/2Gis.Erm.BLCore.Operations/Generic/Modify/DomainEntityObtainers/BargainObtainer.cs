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
    public sealed class BargainObtainer : IBusinessModelEntityObtainer<Bargain>, IAggregateReadModel<Bargain>
    {
        private readonly IFinder _finder;

        public BargainObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Bargain ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BargainDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<Bargain>(dto.Id)).SingleOrDefault() ??
                         new Bargain { IsActive = true };

            entity.Comment = dto.Comment;
            entity.SignedOn = dto.SignedOn;
            entity.ClosedOn = dto.ClosedOn;
            entity.HasDocumentsDebt = (byte)dto.HasDocumentsDebt;
            entity.DocumentsComment = dto.DocumentsComment;
            
            return entity;
        }
    }
}