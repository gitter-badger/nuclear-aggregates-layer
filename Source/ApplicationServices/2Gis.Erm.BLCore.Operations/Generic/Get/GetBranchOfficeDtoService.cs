using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetBranchOfficeDtoService : GetDomainEntityDtoServiceBase<BranchOffice>
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetBranchOfficeDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BranchOffice> GetDto(long entityId)
        {
            return _finder.Find<BranchOffice>(x => x.Id == entityId)
                          .Select(entity => new BranchOfficeDomainEntityDto
                              {
                                  Id = entity.Id,
                                  DgppId = entity.DgppId,
                                  Name = entity.Name,
                                  Inn = entity.Inn,
                                  Ic = entity.Ic,
                                  Annotation = entity.Annotation,
                                  BargainTypeRef = new EntityReference { Id = entity.BargainTypeId, Name = entity.BargainType.Name },
                                  ContributionTypeRef = new EntityReference { Id = entity.ContributionTypeId, Name = entity.ContributionType.Name },
                                  LegalAddress = entity.LegalAddress,
                                  UsnNotificationText = entity.UsnNotificationText,
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn
                              })
                          .Single();
        }

        protected override IDomainEntityDto<BranchOffice> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new BranchOfficeDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}