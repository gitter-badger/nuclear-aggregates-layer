using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public sealed class GetBargainDtoService : GetDomainEntityDtoServiceBase<Bargain>
    {
        private readonly ISecureFinder _finder;

        public GetBargainDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Bargain> GetDto(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Bargain>(entityId))
                          .Select(entity => new BargainDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Number = entity.Number,
                                  BargainTypeRef = new EntityReference { Id = entity.BargainTypeId, Name = entity.BargainType.Name },
                                  LegalPersonRef = new EntityReference { Id = entity.CustomerLegalPersonId, Name = entity.LegalPerson.LegalName },
                                  BranchOfficeOrganizationUnitRef = new EntityReference { Id = entity.ExecutorBranchOfficeId, Name = entity.BranchOfficeOrganizationUnit.ShortLegalName },
                                  Comment = entity.Comment,
                                  SignedOn = entity.SignedOn,
                                  ClosedOn = entity.ClosedOn,
                                  HasDocumentsDebt = (DocumentsDebt)entity.HasDocumentsDebt,
                                  DocumentsComment = entity.DocumentsComment,
                                  OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn,
                                  Timestamp = entity.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Bargain> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            throw new NotSupportedException("Creation of Bargain is not allowed");
        }
    }
}