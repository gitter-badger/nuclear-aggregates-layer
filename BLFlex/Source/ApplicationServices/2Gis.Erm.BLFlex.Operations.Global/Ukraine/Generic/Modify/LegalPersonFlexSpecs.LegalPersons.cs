using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify
{
    public partial class LegalPersonFlexSpecs
    {
        public static class LegalPersons
        {
            public static class Ukraine
            {
                public static class Project
                {
                    public static MapSpecification<LegalPerson, UkraineLegalPersonDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<LegalPerson, UkraineLegalPersonDomainEntityDto>(
                            x =>
                                {
                                    var dto = new UkraineLegalPersonDomainEntityDto
                                        {
                                            Id = x.Id,
                                            LegalName = x.LegalName,
                                            ShortName = x.ShortName,
                                            LegalPersonTypeEnum = x.LegalPersonTypeEnum,
                                            LegalAddress = x.LegalAddress,
                                            PassportSeries = x.PassportSeries,
                                            PassportNumber = x.PassportNumber,
                                            PassportIssuedBy = x.PassportIssuedBy,
                                            RegistrationAddress = x.RegistrationAddress,
                                            Inn = x.Inn,
                                            ClientRef = new EntityReference { Id = x.ClientId, Name = null },
                                            ReplicationCode = x.ReplicationCode,
                                            Comment = x.Comment,
                                            Egrpou = x.Within<UkraineLegalPersonPart>().GetPropertyValue(part => part.Egrpou),
                                            TaxationType = x.Within<UkraineLegalPersonPart>().GetPropertyValue(part => part.TaxationType),
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
                    public static IAssignSpecification<UkraineLegalPersonDomainEntityDto, LegalPerson> Entity()
                    {
                        return new AssignSpecification<UkraineLegalPersonDomainEntityDto, LegalPerson>(
                            (dto, legalPerson) =>
                                {
                                    legalPerson.LegalPersonTypeEnum = dto.LegalPersonTypeEnum;
                                    legalPerson.LegalName = dto.LegalName;
                                    legalPerson.ShortName = dto.ShortName;
                                    legalPerson.LegalAddress = dto.LegalAddress;
                                    legalPerson.PassportNumber = dto.PassportNumber;
                                    legalPerson.RegistrationAddress = dto.RegistrationAddress;
                                    legalPerson.Inn = dto.Inn;
                                    legalPerson.PassportSeries = dto.PassportSeries;
                                    legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
                                    legalPerson.ClientId = dto.ClientRef.Id;
                                    legalPerson.Comment = dto.Comment;
                                    legalPerson.PassportIssuedBy = dto.PassportIssuedBy;
                                    legalPerson.Within<UkraineLegalPersonPart>().SetPropertyValue(part => part.Egrpou, dto.Egrpou);
                                    legalPerson.Within<UkraineLegalPersonPart>().SetPropertyValue(part => part.TaxationType, dto.TaxationType);
                                    legalPerson.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}