using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Czech;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static class Czech
            {
                public static class Project
                {
                    public static IProjectSpecification<BranchOfficeOrganizationUnit, CzechBranchOfficeOrganizationUnitDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<BranchOfficeOrganizationUnit, CzechBranchOfficeOrganizationUnitDomainEntityDto>(
                            x =>
                                {
                                    var dto = new CzechBranchOfficeOrganizationUnitDomainEntityDto
                                        {
                                            Id = x.Id,
                                            OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = null },
                                            BranchOfficeRef = new EntityReference { Id = x.BranchOfficeId, Name = null },
                                            ApplicationCityName = x.ApplicationCityName,
                                            FullNameInGenitive = x.ChiefNameInGenitive,
                                            FullNameInNominative = x.ChiefNameInNominative,
                                            Registered = x.Registered,
                                            IsPrimary = x.IsPrimary,
                                            IsPrimaryForRegionalSales = x.IsPrimaryForRegionalSales,
                                            PhoneNumber = x.PhoneNumber,
                                            Email = x.Email,
                                            PositionInGenitive = x.PositionInGenitive,
                                            PositionInNominative = x.PositionInNominative,
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
                    public static IAssignSpecification<CzechBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> Entity()
                    {
                        return new AssignSpecification<CzechBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit>(
                            (dto, entity) =>
                                {
                                    entity.Id = dto.Id;
                                    entity.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
                                    entity.BranchOfficeId = dto.BranchOfficeRef.Id.Value;
                                    entity.ApplicationCityName = dto.ApplicationCityName;
                                    entity.ChiefNameInGenitive = dto.FullNameInGenitive;
                                    entity.ChiefNameInNominative = dto.FullNameInNominative;
                                    entity.PositionInGenitive = dto.PositionInGenitive;
                                    entity.PositionInNominative = dto.PositionInNominative;
                                    entity.Registered = dto.Registered;
                                    entity.ActualAddress = dto.ActualAddress;
                                    entity.PostalAddress = dto.PostalAddress;
                                    entity.PaymentEssentialElements = dto.PaymentEssentialElements;
                                    entity.PhoneNumber = dto.PhoneNumber;
                                    entity.ShortLegalName = dto.ShortLegalName;
                                    entity.RegistrationCertificate = dto.RegistrationCertificate;
                                    entity.Email = dto.Email;
                                    entity.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}
