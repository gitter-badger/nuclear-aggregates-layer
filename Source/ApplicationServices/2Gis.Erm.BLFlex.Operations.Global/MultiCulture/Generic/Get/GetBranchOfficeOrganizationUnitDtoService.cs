using System.Linq;

using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetBranchOfficeOrganizationUnitDtoService : GetDomainEntityDtoServiceBase<BranchOfficeOrganizationUnit>, IRussiaAdapted, ICzechAdapted, ICyprusAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetBranchOfficeOrganizationUnitDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> GetDto(long entityId)
        {
            return _finder.Find<BranchOfficeOrganizationUnit>(x => x.Id == entityId)
                          .Select(entity => new BranchOfficeOrganizationUnitDomainEntityDto
                              {
                                  Id = entity.Id,
                                  OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
                                  BranchOfficeRef = new EntityReference { Id = entity.BranchOfficeId, Name = entity.BranchOffice.Name },
                                  ChiefNameInGenitive = entity.ChiefNameInGenitive,
                                  ChiefNameInNominative = entity.ChiefNameInNominative,
                                  Registered = entity.Registered,
                                  IsPrimary = entity.IsPrimary,
                                  IsPrimaryForRegionalSales = entity.IsPrimaryForRegionalSales,
                                  OperatesOnTheBasisInGenitive = entity.OperatesOnTheBasisInGenitive,
                                  Kpp = entity.Kpp,
                                  PhoneNumber = entity.PhoneNumber,
                                  Email = entity.Email,
                                  PositionInGenitive = entity.PositionInGenitive,
                                  PositionInNominative = entity.PositionInNominative,
                                  ShortLegalName = entity.ShortLegalName,
                                  ActualAddress = entity.ActualAddress,
                                  PostalAddress = entity.PostalAddress,
                                  BranchOfficeAddlId = entity.BranchOffice.Id,
                                  BranchOfficeAddlIc = entity.BranchOffice.Ic,
                                  BranchOfficeAddlInn = entity.BranchOffice.Inn,
                                  BranchOfficeAddlLegalAddress = entity.BranchOffice.LegalAddress,
                                  BranchOfficeAddlName = entity.BranchOffice.Name,
                                  PaymentEssentialElements = entity.PaymentEssentialElements,
                                  SyncCode1C = entity.SyncCode1C,
                                  RegistrationCertificate = entity.RegistrationCertificate,
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

        protected override IDomainEntityDto<BranchOfficeOrganizationUnit> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new BranchOfficeOrganizationUnitDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };

            switch (parentEntityName)
            {
                case EntityName.BranchOffice:
                    {
                        dto.BranchOfficeRef = new EntityReference { Id = parentEntityId.Value, Name = _finder.Find<BranchOffice>(x => x.Id == parentEntityId).Select(x => x.Name).SingleOrDefault() };
                        dto.ShortLegalName = dto.BranchOfficeRef.Name;
                    }

                    break;
                case EntityName.OrganizationUnit:
                    {
                        dto.OrganizationUnitRef = new EntityReference { Id = parentEntityId.Value, Name = _finder.Find<OrganizationUnit>(x => x.Id == parentEntityId).Select(x => x.Name).SingleOrDefault() };
                    }

                    break;
            }

            return dto;
        }
    }
}