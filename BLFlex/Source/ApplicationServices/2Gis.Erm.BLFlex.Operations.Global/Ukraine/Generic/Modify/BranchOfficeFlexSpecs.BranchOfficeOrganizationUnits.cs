using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static class Ukraine
            {
                public static class Project
                {
                    public static IProjectSpecification<BranchOfficeOrganizationUnit, UkraineBranchOfficeOrganizationUnitDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<BranchOfficeOrganizationUnit, UkraineBranchOfficeOrganizationUnitDomainEntityDto>(
                            x =>
                                {
                                    var dto = new UkraineBranchOfficeOrganizationUnitDomainEntityDto
                                        {
                                            Id = x.Id,
                                            OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = null },
                                            BranchOfficeRef = new EntityReference { Id = x.BranchOfficeId, Name = null },
                                            ChiefNameInGenitive = x.ChiefNameInGenitive,
                                            ChiefNameInNominative = x.ChiefNameInNominative,
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
                                            OperatesOnTheBasisInGenitive = x.OperatesOnTheBasisInGenitive,
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
                    public static IAssignSpecification<UkraineBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> Entity()
                    {
                        return new AssignSpecification<UkraineBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit>(
                            (dto, entity) =>
                            {
                                entity.Id = dto.Id;
                                entity.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
                                entity.BranchOfficeId = dto.BranchOfficeRef.Id.Value;
                                entity.ChiefNameInGenitive = dto.ChiefNameInGenitive;
                                entity.ChiefNameInNominative = dto.ChiefNameInNominative;
                                entity.OperatesOnTheBasisInGenitive = dto.OperatesOnTheBasisInGenitive;
                                entity.ActualAddress = dto.ActualAddress;
                                entity.PostalAddress = dto.PostalAddress;
                                entity.PaymentEssentialElements = dto.PaymentEssentialElements;
                                entity.PhoneNumber = dto.PhoneNumber;
                                entity.PositionInGenitive = dto.PositionInGenitive;
                                entity.PositionInNominative = dto.PositionInNominative;
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