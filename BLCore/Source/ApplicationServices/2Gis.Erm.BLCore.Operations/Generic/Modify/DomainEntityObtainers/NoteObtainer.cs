﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class NoteObtainer : ISimplifiedModelEntityObtainer<Note>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;

        public NoteObtainer(IFinder finder, IUserContext userContext)
        {
            _finder = finder;
            _userContext = userContext;
        }

        public Note ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (NoteDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<Note>(dto.Id)).One()
                ?? new Note { OwnerCode = _userContext.Identity.Code };

            entity.Id = dto.Id;
            entity.FileId = dto.FileId;
            entity.ParentId = dto.ParentRef.Id.Value;
            entity.Title = dto.Title;
            entity.Text = dto.Text;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}