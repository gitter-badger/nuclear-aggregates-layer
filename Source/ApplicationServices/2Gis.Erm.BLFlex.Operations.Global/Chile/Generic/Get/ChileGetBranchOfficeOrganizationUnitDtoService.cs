using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public class ChileGetBranchOfficeOrganizationUnitDtoService : GetDomainEntityDtoServiceBase<BranchOfficeOrganizationUnit>, IChileAdapted
    {
        private readonly ISecureFinder _securefinder;
        private readonly IFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;
        private readonly IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart> _boouPropertiesConverter;

        public ChileGetBranchOfficeOrganizationUnitDtoService(IUserContext userContext, ISecureFinder securefinder, IFinder finder, IAPIIdentityServiceSettings identityServiceSettings,
            IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart> boouPropertiesConverter)
            : base(userContext)
        {
            _securefinder = securefinder;
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
            _boouPropertiesConverter = boouPropertiesConverter;
        }

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> GetDto(long entityId)
        {
            var entity = GetBOOU(entityId);

            var organizationUnitName = _securefinder.Find<OrganizationUnit>(x => x.Id == entity.OrganizationUnitId).Select(x => x.Name).Single();
            var branchOffice = _securefinder.Find<BranchOffice>(x => x.Id == entity.BranchOfficeId).Single();
            var boouPart = (ChileBranchOfficeOrganizationUnitPart)entity.Parts.Single();

            return new ChileBranchOfficeOrganizationUnitDomainEntityDto
            {
                Id = entity.Id,
                OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = organizationUnitName },
                BranchOfficeRef = new EntityReference { Id = entity.BranchOfficeId, Name = branchOffice.Name },
                ChiefNameInNominative = entity.ChiefNameInNominative,
                IsPrimary = entity.IsPrimary,
                IsPrimaryForRegionalSales = entity.IsPrimaryForRegionalSales,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                PositionInNominative = entity.PositionInNominative,
                RepresentativeRut = boouPart.RepresentativeRut,
                ShortLegalName = entity.ShortLegalName,
                ActualAddress = entity.ActualAddress,
                PostalAddress = entity.PostalAddress,
                BranchOfficeAddlId = branchOffice.Id,
                BranchOfficeAddlRut = branchOffice.Inn,
                BranchOfficeAddlLegalAddress = branchOffice.LegalAddress,
                BranchOfficeAddlName = branchOffice.Name,
                PaymentEssentialElements = entity.PaymentEssentialElements,
                RegistrationCertificate = entity.RegistrationCertificate,
                OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                CreatedOn = entity.CreatedOn,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                ModifiedOn = entity.ModifiedOn,
                Timestamp = entity.Timestamp
            };
        }

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new ChileBranchOfficeOrganizationUnitDomainEntityDto
            {
                IdentityServiceUrl = _identityServiceSettings.RestUrl
            };

            switch (parentEntityName)
            {
                case EntityName.BranchOffice:
                    {
                        dto.BranchOfficeRef = new EntityReference { Id = parentEntityId.Value, Name = _securefinder.Find<BranchOffice>(x => x.Id == parentEntityId).Select(x => x.Name).SingleOrDefault() };
                        dto.ShortLegalName = dto.BranchOfficeRef.Name;
                    }

                    break;
                case EntityName.OrganizationUnit:
                    {
                        dto.OrganizationUnitRef = new EntityReference { Id = parentEntityId.Value, Name = _securefinder.Find<OrganizationUnit>(x => x.Id == parentEntityId).Select(x => x.Name).SingleOrDefault() };
                    }

                    break;
            }

            return dto;
        }

        private BranchOfficeOrganizationUnit GetBOOU(long entityId)
        {
            var entityInstanceDto = GetBusinessEntityInstanceDtoQuery(entityId, BusinessModel.Chile).SingleOrDefault();
            var boou = _securefinder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(entityId)).Single();

            if (entityInstanceDto != null)
            {
                var boouPart = _boouPropertiesConverter.ConvertFromDynamicEntityInstance(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                boou.Parts = new List<ChileBranchOfficeOrganizationUnitPart> { boouPart };
            }
            else
            {
                boou.Parts = new List<ChileBranchOfficeOrganizationUnitPart> { new ChileBranchOfficeOrganizationUnitPart() };
            }

            return boou;
        }

        private IQueryable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDtoQuery(long entityId, BusinessModel businessModel)
        {
            var findSpec = BusinessEntitySpecs.BusinessEntity.Find.ByReferencedEntity(entityId)/* &&
                           BusinessEntitySpecs.BusinessEntity.Find.ByBusinessModel(businessModel)*/;

            return _finder.Find<BusinessEntityInstance, BusinessEntityInstanceDto>(BusinessEntitySpecs.BusinessEntity.Select.BusinessEntityInstanceDto(), findSpec);
        }

    }
}