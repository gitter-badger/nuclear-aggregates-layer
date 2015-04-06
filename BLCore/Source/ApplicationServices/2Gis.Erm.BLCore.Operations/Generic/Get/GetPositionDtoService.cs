using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPositionDtoService : GetDomainEntityDtoServiceBase<Position>
    {
        private readonly ISecureFinder _finder;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPositionRepository _positionRepository;
        private readonly IUserContext _userContext;

        public GetPositionDtoService(IUserContext userContext,
                                     ISecureFinder finder,
                                     IPositionRepository positionRepository,
                                     ISecurityServiceFunctionalAccess functionalAccessService) : base(userContext)
        {
            _finder = finder;
            _positionRepository = positionRepository;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Position> GetDto(long entityId)
        {
            var dto = _finder.Find<Position>(x => x.Id == entityId)
                             .Select(entity => new PositionDomainEntityDto
                                 {
                                     Id = entity.Id,
                                     DgppId = entity.DgppId,
                                     ExportCode = entity.ExportCode,
                                     Name = entity.Name,
                                     IsComposite = entity.IsComposite,
                                     BindingObjectTypeEnum = entity.BindingObjectTypeEnum,
                                     SalesModel = entity.SalesModel,
                                     PositionsGroup = entity.PositionsGroup,
                                     CalculationMethodEnum = entity.CalculationMethodEnum,
                                     IsControlledByAmount = entity.IsControlledByAmount,
                                     PlatformRef = new EntityReference { Id = entity.PlatformId, Name = entity.Platform.Name },
                                     CategoryRef = new EntityReference { Id = entity.CategoryId, Name = entity.PositionCategory.Name },
                                     AdvertisementTemplateRef =
                                         new EntityReference { Id = entity.AdvertisementTemplateId, Name = entity.AdvertisementTemplate.Name },
                                     Timestamp = entity.Timestamp,
                                     CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                     CreatedOn = entity.CreatedOn,
                                     IsActive = entity.IsActive,
                                     IsDeleted = entity.IsDeleted,
                                     ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                     ModifiedOn = entity.ModifiedOn,
                                     RestrictChildPositionPlatforms = entity.RestrictChildPositionPlatforms
                                 })
                             .Single();

            dto.IsPublished = _positionRepository.IsInPublishedPrices(dto.Id);
            dto.RestrictChildPositionPlatformsCanBeChanged =
                _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.PositionAdministrationCode, _userContext.Identity.Code);

            return dto;
        }

        protected override IDomainEntityDto<Position> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new PositionDomainEntityDto
                {
                    RestrictChildPositionPlatformsCanBeChanged =
                        _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.PositionAdministrationCode, _userContext.Identity.Code),
                    SalesModel = SalesModel.GuaranteedProvision
                };
        }
    }
}