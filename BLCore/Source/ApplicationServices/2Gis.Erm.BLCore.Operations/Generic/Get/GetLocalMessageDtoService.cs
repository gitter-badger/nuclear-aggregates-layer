﻿using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetLocalMessageDtoService : GetDomainEntityDtoServiceBase<LocalMessage>
    {
        private readonly ISecureFinder _finder;

        public GetLocalMessageDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<LocalMessage> GetDto(long entityId)
        {
            return _finder.Find<LocalMessage>(x => x.Id == entityId)
                          .Select(entity => new LocalMessageDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Status = (LocalMessageStatus)entity.Status,
                                  IntegrationTypeImport = (IntegrationTypeImport)entity.MessageType.IntegrationType,
                                  IntegrationTypeExport = (IntegrationTypeExport)entity.MessageType.IntegrationType,
                                  SenderSystem = (IntegrationSystem)entity.MessageType.SenderSystem,
                                  ReceiverSystem = (IntegrationSystem)entity.MessageType.ReceiverSystem,
                                  ProcessResult = entity.ProcessResult,
                                  EventDate = entity.EventDate,
                                  OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
                                  IsDeleted = entity.IsDeleted,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  ModifiedOn = entity.ModifiedOn,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  Timestamp = entity.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<LocalMessage> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            throw new NotSupportedException("Local messages should be created through Export feature");
        }
    }
}