using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static class Chile
            {
                public static class Project
                {
                    public static MapSpecification<BranchOfficeOrganizationUnit, ChileBranchOfficeOrganizationUnitDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<BranchOfficeOrganizationUnit, ChileBranchOfficeOrganizationUnitDomainEntityDto>(
                            x =>
                                {
                                    var dto = new ChileBranchOfficeOrganizationUnitDomainEntityDto
                                        {
                                            Id = x.Id,
                                            OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = null },
                                            BranchOfficeRef = new EntityReference { Id = x.BranchOfficeId, Name = null },
                                            ApplicationCityName = x.ApplicationCityName,
                                            RepresentativeName = x.ChiefNameInNominative,
                                            RepresentativeRut = x.Within<ChileBranchOfficeOrganizationUnitPart>().GetPropertyValue(part => part.RepresentativeRut),
                                            IsPrimary = x.IsPrimary,
                                            IsPrimaryForRegionalSales = x.IsPrimaryForRegionalSales,
                                            PhoneNumber = x.PhoneNumber,
                                            Email = x.Email,
                                            RepresentativePosition = x.PositionInNominative,
                                            ShortLegalName = x.ShortLegalName,
                                            ActualAddress = x.ActualAddress,
                                            PostalAddress = x.PostalAddress,
                                            PaymentEssentialElements = x.PaymentEssentialElements,
                                            RegistrationCertificate = x.RegistrationCertificate,
                                            OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn,
                                            Timestamp = x.Timestamp
                                        };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<ChileBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> Entity()
                    {
                        return new AssignSpecification<ChileBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit>(
                            (dto, entity) =>
                                {
                                    entity.Id = dto.Id;
                                    entity.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
                                    entity.BranchOfficeId = dto.BranchOfficeRef.Id.Value;
                                    entity.ApplicationCityName = dto.ApplicationCityName;
                                    entity.ChiefNameInNominative = dto.RepresentativeName;
                                    entity.ActualAddress = dto.ActualAddress;
                                    entity.PostalAddress = dto.PostalAddress;
                                    entity.PaymentEssentialElements = dto.PaymentEssentialElements;
                                    entity.PhoneNumber = dto.PhoneNumber;
                                    entity.PositionInNominative = dto.RepresentativePosition;
                                    entity.ShortLegalName = dto.ShortLegalName;
                                    entity.RegistrationCertificate = dto.RegistrationCertificate;
                                    entity.Email = dto.Email;
                                    entity.Within<ChileBranchOfficeOrganizationUnitPart>().SetPropertyValue(part => part.RepresentativeRut, dto.RepresentativeRut);
                                    entity.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}
