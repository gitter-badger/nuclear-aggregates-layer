using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public static partial class LegalPersonFlexSpecs
    {
        public static class LegalPersons
        {
            public static class Chile
            {
                public static class Project
                {
                    public static ProjectSpecification<LegalPerson, ChileLegalPersonDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<LegalPerson, ChileLegalPersonDomainEntityDto>(
                            x =>
                            {
                                if (x.IsNew())
                                {
                                    return new ChileLegalPersonDomainEntityDto
                                        {
                                            ClientRef = x.ClientId != 0
                                                            ? new EntityReference { Id = x.ClientId, Name = null }
                                                            : new EntityReference(),
                                        };
                                }

                                var communeId = x.Within<ChileLegalPersonPart>().GetPropertyValue(part => part.CommuneId);
                                var dto = new ChileLegalPersonDomainEntityDto
                                    {
                                        Id = x.Id,
                                        LegalName = x.LegalName,
                                        LegalPersonTypeEnum = x.LegalPersonTypeEnum,
                                        LegalAddress = x.LegalAddress,
                                        PassportSeries = x.PassportSeries,
                                        PassportNumber = x.PassportNumber,
                                        PassportIssuedBy = x.PassportIssuedBy,
                                        RegistrationAddress = x.RegistrationAddress,
                                        Rut = x.Inn,
                                        ClientRef = new EntityReference { Id = x.ClientId, Name = null },
                                        ReplicationCode = x.ReplicationCode,
                                        Comment = x.Comment,
                                        CommuneRef = new EntityReference { Id = communeId, Name = null },
                                        OperationsKind = x.Within<ChileLegalPersonPart>().GetPropertyValue(part => part.OperationsKind),
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
                    public static IAssignSpecification<ChileLegalPersonDomainEntityDto, LegalPerson> Entity()
                    {
                        return new AssignSpecification<ChileLegalPersonDomainEntityDto, LegalPerson>(
                            (dto, legalPerson) =>
                                {
                                    legalPerson.LegalPersonTypeEnum = dto.LegalPersonTypeEnum;
                                    legalPerson.LegalName = dto.LegalName;
                                    legalPerson.LegalAddress = dto.LegalAddress;
                                    legalPerson.PassportNumber = dto.PassportNumber;
                                    legalPerson.RegistrationAddress = dto.RegistrationAddress;
                                    legalPerson.Inn = dto.Rut;
                                    legalPerson.PassportSeries = dto.PassportSeries;
                                    legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
                                    legalPerson.ClientId = dto.ClientRef.Id;
                                    legalPerson.Comment = dto.Comment;
                                    legalPerson.PassportIssuedBy = dto.PassportIssuedBy;
                                    legalPerson.Timestamp = dto.Timestamp;
                                    legalPerson.Within<ChileLegalPersonPart>().SetPropertyValue(part => part.OperationsKind, dto.OperationsKind);
                                    legalPerson.Within<ChileLegalPersonPart>().SetPropertyValue(part => part.CommuneId, dto.CommuneRef.Id.Value);
                                });
                    }
                }
            }
        }
    }
}