﻿using System.Linq;

using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetClientDtoService : GetDomainEntityDtoServiceBase<Client>, IRussiaAdapted
    {
        private readonly ISecureFinder _finder;

        public GetClientDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Client> GetDto(long entityId)
        {
            var modelDto = _finder.Find<Client>(x => x.Id == entityId)
                                  .Select(entity => new ClientDomainEntityDto
                                      {
                                          Id = entity.Id,
                                          Name = entity.Name,
                                          MainPhoneNumber = entity.MainPhoneNumber,
                                          AdditionalPhoneNumber1 = entity.AdditionalPhoneNumber1,
                                          AdditionalPhoneNumber2 = entity.AdditionalPhoneNumber2,
                                          Email = entity.Email,
                                          Fax = entity.Fax,
                                          Website = entity.Website,
                                          InformationSource = (InformationSource)entity.InformationSource,
                                          ReplicationCode = entity.ReplicationCode,
                                          PromisingValue = entity.PromisingValue,
                                          Comment = entity.Comment,
                                          MainAddress = entity.MainAddress,
                                          LastQualifyTime = entity.LastQualifyTime,
                                          LastDisqualifyTime = entity.LastDisqualifyTime,
                                          MainFirmRef = new EntityReference { Id = entity.MainFirmId, Name = entity.Firm.Name },
                                          TerritoryRef = new EntityReference { Id = entity.TerritoryId, Name = entity.Territory.Name },
                                          IsAdvertisingAgency = entity.IsAdvertisingAgency,
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

            return modelDto;
        }

        protected override IDomainEntityDto<Client> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new ClientDomainEntityDto();
        }
    }
}