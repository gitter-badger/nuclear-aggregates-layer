using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify
{
    public static class BranchOfficeFlexSpecs
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static class Emirates
            {
                public static class Project
                {
                    public static MapSpecification<BranchOfficeOrganizationUnit, EmiratesBranchOfficeOrganizationUnitDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<BranchOfficeOrganizationUnit, EmiratesBranchOfficeOrganizationUnitDomainEntityDto>(
                            x =>
                                {
                                    var dto = new EmiratesBranchOfficeOrganizationUnitDomainEntityDto
                                        {
                                            Id = x.Id,
                                            OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = null },
                                            BranchOfficeRef = new EntityReference { Id = x.BranchOfficeId, Name = null },
                                            ApplicationCityName = x.ApplicationCityName,
                                            ChiefNameInNominative = x.ChiefNameInNominative,
                                            IsPrimary = x.IsPrimary,
                                            IsPrimaryForRegionalSales = x.IsPrimaryForRegionalSales,
                                            PhoneNumber = x.PhoneNumber,
                                            Email = x.Email,
                                            PositionInNominative = x.PositionInNominative,
                                            ShortLegalName = x.ShortLegalName,
                                            ActualAddress = x.ActualAddress,
                                            PostalAddress = x.PostalAddress,
                                            PaymentEssentialElements = x.PaymentEssentialElements,
                                            OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn,
                                            Timestamp = x.Timestamp,
                                            Fax = x.Within<EmiratesBranchOfficeOrganizationUnitPart>().GetPropertyValue(part => part.Fax)
                                        };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<EmiratesBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> Entity()
                    {
                        return new AssignSpecification<EmiratesBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit>(
                            (dto, branchOfficeOrganizationUnit) =>
                                {
                                    branchOfficeOrganizationUnit.Id = dto.Id;
                                    branchOfficeOrganizationUnit.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
                                    branchOfficeOrganizationUnit.BranchOfficeId = dto.BranchOfficeRef.Id.Value;
                                    branchOfficeOrganizationUnit.ApplicationCityName = dto.ApplicationCityName;
                                    branchOfficeOrganizationUnit.ChiefNameInNominative = dto.ChiefNameInNominative;
                                    branchOfficeOrganizationUnit.ActualAddress = dto.ActualAddress;
                                    branchOfficeOrganizationUnit.PostalAddress = dto.PostalAddress;
                                    branchOfficeOrganizationUnit.PaymentEssentialElements = dto.PaymentEssentialElements;
                                    branchOfficeOrganizationUnit.PhoneNumber = dto.PhoneNumber;
                                    branchOfficeOrganizationUnit.PositionInNominative = dto.PositionInNominative;
                                    branchOfficeOrganizationUnit.ShortLegalName = dto.ShortLegalName;
                                    branchOfficeOrganizationUnit.Email = dto.Email;
                                    branchOfficeOrganizationUnit.Timestamp = dto.Timestamp;

                                    branchOfficeOrganizationUnit.Within<EmiratesBranchOfficeOrganizationUnitPart>()
                                                                .SetPropertyValue(part => part.Fax, dto.Fax);
                                });
                    }
                }
            }
        }

        public static class BranchOffices
        {
            public static class Emirates
            {
                public static class Project
                {
                    public static MapSpecification<BranchOffice, EmiratesBranchOfficeDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<BranchOffice, EmiratesBranchOfficeDomainEntityDto>(
                            x =>
                                {
                                    var dto = new EmiratesBranchOfficeDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            CommercialLicense = x.Inn,
                                            BargainTypeRef = new EntityReference { Id = x.BargainTypeId, Name = null },
                                            ContributionTypeRef = new EntityReference { Id = x.ContributionTypeId, Name = null },
                                            LegalAddress = x.LegalAddress,
                                            Timestamp = x.Timestamp,
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn
                                        };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<EmiratesBranchOfficeDomainEntityDto, BranchOffice> Entity()
                    {
                        return new AssignSpecification<EmiratesBranchOfficeDomainEntityDto, BranchOffice>(
                            (dto, entity) =>
                                {
                                    entity.Id = dto.Id;
                                    entity.Name = dto.Name;
                                    entity.Inn = dto.CommercialLicense;
                                    entity.BargainTypeId = dto.BargainTypeRef.Id;
                                    entity.ContributionTypeId = dto.ContributionTypeRef.Id;
                                    entity.LegalAddress = dto.LegalAddress;
                                    entity.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}