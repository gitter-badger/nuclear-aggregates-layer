using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static class Russia
            {
                public static class Project
                {
                    public static IProjectSpecification<BranchOfficeOrganizationUnit, RussiaBranchOfficeOrganizationUnitDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<BranchOfficeOrganizationUnit, RussiaBranchOfficeOrganizationUnitDomainEntityDto>(
                            x =>
                                {
                                    var dto = new RussiaBranchOfficeOrganizationUnitDomainEntityDto
                                        {
                                            Id = x.Id,
                                            OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = null },
                                            BranchOfficeRef = new EntityReference { Id = x.BranchOfficeId, Name = null },
                                            ApplicationCityName = x.ApplicationCityName,
                                            ChiefNameInGenitive = x.ChiefNameInGenitive,
                                            ChiefNameInNominative = x.ChiefNameInNominative,
                                            IsPrimary = x.IsPrimary,
                                            IsPrimaryForRegionalSales = x.IsPrimaryForRegionalSales,
                                            OperatesOnTheBasisInGenitive = x.OperatesOnTheBasisInGenitive,
                                            Kpp = x.Kpp,
                                            PhoneNumber = x.PhoneNumber,
                                            Email = x.Email,
                                            PositionInGenitive = x.PositionInGenitive,
                                            PositionInNominative = x.PositionInNominative,
                                            ShortLegalName = x.ShortLegalName,
                                            ActualAddress = x.ActualAddress,
                                            PostalAddress = x.PostalAddress,
                                            PaymentEssentialElements = x.PaymentEssentialElements,
                                            SyncCode1C = x.SyncCode1C,
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
                    public static IAssignSpecification<RussiaBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> Entity()
                    {
                        return new AssignSpecification<RussiaBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit>(
                            (dto, entity) =>
                            {
                                entity.Id = dto.Id;
                                entity.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
                                entity.BranchOfficeId = dto.BranchOfficeRef.Id.Value;
                                entity.ApplicationCityName = dto.ApplicationCityName;
                                entity.ChiefNameInGenitive = dto.ChiefNameInGenitive;
                                entity.ChiefNameInNominative = dto.ChiefNameInNominative;
                                entity.OperatesOnTheBasisInGenitive = dto.OperatesOnTheBasisInGenitive;
                                entity.Kpp = dto.Kpp;
                                entity.ActualAddress = dto.ActualAddress;
                                entity.PostalAddress = dto.PostalAddress;
                                entity.PaymentEssentialElements = dto.PaymentEssentialElements;
                                entity.PhoneNumber = dto.PhoneNumber;
                                entity.PositionInGenitive = dto.PositionInGenitive;
                                entity.PositionInNominative = dto.PositionInNominative;
                                entity.ShortLegalName = dto.ShortLegalName;
                                entity.SyncCode1C = dto.SyncCode1C;
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