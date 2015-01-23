using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetClientDtoService : GetDomainEntityDtoServiceBase<Client>, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted, IKazakhstanAdapted
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
                                  .Select(entity => new MultiCultureClientDomainEntityDto
                                      {
                                          Id = entity.Id,
                                          Name = entity.Name,
                                          MainPhoneNumber = entity.MainPhoneNumber,
                                          AdditionalPhoneNumber1 = entity.AdditionalPhoneNumber1,
                                          AdditionalPhoneNumber2 = entity.AdditionalPhoneNumber2,
                                          Email = entity.Email,
                                          Fax = entity.Fax,
                                          Website = entity.Website,
                                          InformationSource = entity.InformationSource,
                                          Comment = entity.Comment,
                                          MainAddress = entity.MainAddress,
                                          LastQualifyTime = entity.LastQualifyTime,
                                          LastDisqualifyTime = entity.LastDisqualifyTime,
                                          MainFirmRef = new EntityReference { Id = entity.MainFirmId, Name = entity.Firm.Name },
                                          TerritoryRef = new EntityReference { Id = entity.TerritoryId, Name = entity.Territory.Name },
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

            modelDto.LastDisqualifyTime = modelDto.LastDisqualifyTime.AssumeUtcKind();
            modelDto.LastQualifyTime = modelDto.LastQualifyTime.AssumeUtcKind();

            return modelDto;
        }

        protected override IDomainEntityDto<Client> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new MultiCultureClientDomainEntityDto { LastQualifyTime = DateTime.UtcNow };
        }
    }
}