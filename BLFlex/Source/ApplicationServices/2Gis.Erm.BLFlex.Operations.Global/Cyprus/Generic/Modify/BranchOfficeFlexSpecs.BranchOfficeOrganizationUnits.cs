using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Cyprus;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static class Cyprus
            {
                public static class Project
                {
                    public static IProjectSpecification<BranchOfficeOrganizationUnit, CyprusBranchOfficeOrganizationUnitDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<BranchOfficeOrganizationUnit, CyprusBranchOfficeOrganizationUnitDomainEntityDto>(
                            x =>
                                {
                                    var dto = new CyprusBranchOfficeOrganizationUnitDomainEntityDto
                                        {
                                            Id = x.Id,
                                            OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = null },
                                            BranchOfficeRef = new EntityReference { Id = x.BranchOfficeId, Name = null },
                                            ApplicationCityName = x.ApplicationCityName,
                                            ChiefNameInNominative = x.ChiefNameInNominative,
                                            IsPrimary = x.IsPrimary,
                                            IsPrimaryForRegionalSales = x.IsPrimaryForRegionalSales,
                                            OperatesOnTheBasisInGenitive = x.OperatesOnTheBasisInGenitive,
                                            PaymentEssentialElements = x.PaymentEssentialElements,
                                            PhoneNumber = x.PhoneNumber,
                                            Email = x.Email,
                                            PositionInNominative = x.PositionInNominative,
                                            ShortLegalName = x.ShortLegalName,
                                            ActualAddress = x.ActualAddress,
                                            PostalAddress = x.PostalAddress,
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
                    public static IAssignSpecification<CyprusBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> Entity()
                    {
                        return new AssignSpecification<CyprusBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit>(
                            (dto, entity) =>
                                {
                                    entity.Id = dto.Id;
                                    entity.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
                                    entity.BranchOfficeId = dto.BranchOfficeRef.Id.Value;
                                    entity.ApplicationCityName = dto.ApplicationCityName;
                                    entity.ActualAddress = dto.ActualAddress;
                                    entity.PostalAddress = dto.PostalAddress;
                                    entity.PhoneNumber = dto.PhoneNumber;
                                    entity.ShortLegalName = dto.ShortLegalName;
                                    entity.RegistrationCertificate = dto.RegistrationCertificate;
                                    entity.OperatesOnTheBasisInGenitive = dto.OperatesOnTheBasisInGenitive;
                                    entity.PaymentEssentialElements = dto.PaymentEssentialElements;
                                    entity.Email = dto.Email;
                                    entity.ChiefNameInNominative = dto.ChiefNameInNominative;
                                    entity.SyncCode1C = dto.SyncCode1C;
                                    entity.PositionInNominative = dto.PositionInNominative;
                                    entity.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}
