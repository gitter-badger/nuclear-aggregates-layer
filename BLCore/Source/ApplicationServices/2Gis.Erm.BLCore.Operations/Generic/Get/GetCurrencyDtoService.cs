﻿using System.Linq;

using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetCurrencyDtoService : GetDomainEntityDtoServiceBase<Currency>
    {
        private readonly IFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetCurrencyDtoService(IUserContext userContext, IFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<Currency> GetDto(long entityId)
        {
            return _finder.Find<Currency>(x => x.Id == entityId)
                          .Select(entity => new CurrencyDomainEntityDto
                              {
                                  Id = entity.Id,
                                  ISOCode = entity.ISOCode,
                                  Name = entity.Name,
                                  Symbol = entity.Symbol,
                                  IsBase = entity.IsBase,
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                  ModifiedOn = entity.ModifiedOn
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Currency> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new CurrencyDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}