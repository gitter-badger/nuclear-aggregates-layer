using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify
{
    public partial class LegalPersonFlexSpecs
    {
        public static class LegalPersons
        {
            public static class Kazakhstan
            {
                public static class Project
                {
                    public static MapSpecification<LegalPerson, KazakhstanLegalPersonDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<LegalPerson, KazakhstanLegalPersonDomainEntityDto>(
                            x =>
                                {
                                    var dto = new KazakhstanLegalPersonDomainEntityDto
                                        {
                                            Id = x.Id,
                                            LegalName = x.LegalName,
                                            ShortName = x.ShortName,
                                            LegalPersonTypeEnum = (LegalPersonType)x.LegalPersonTypeEnum,
                                            LegalAddress = x.LegalAddress,
                                            RegistrationAddress = x.RegistrationAddress,
                                            Bin = x.Inn,
                                            IdentityCardNumber = x.Within<KazakhstanLegalPersonPart>().GetPropertyValue(part => part.IdentityCardNumber),
                                            IdentityCardIssuedBy = x.Within<KazakhstanLegalPersonPart>().GetPropertyValue(part => part.IdentityCardIssuedBy),
                                            IdentityCardIssuedOn = x.Within<KazakhstanLegalPersonPart>().GetPropertyValue(part => part.IdentityCardIssuedOn),
                                            ClientRef = new EntityReference { Id = x.ClientId, Name = null },
                                            ReplicationCode = x.ReplicationCode,
                                            Comment = x.Comment,
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
                    public static IAssignSpecification<KazakhstanLegalPersonDomainEntityDto, LegalPerson> Entity()
                    {
                        return new AssignSpecification<KazakhstanLegalPersonDomainEntityDto, LegalPerson>(
                            (dto, legalPerson) =>
                                {
                                    legalPerson.LegalPersonTypeEnum = dto.LegalPersonTypeEnum;
                                    legalPerson.LegalName = dto.LegalName;
                                    legalPerson.ShortName = dto.ShortName;
                                    legalPerson.LegalAddress = dto.LegalAddress;
                                    legalPerson.RegistrationAddress = dto.RegistrationAddress;
                                    legalPerson.Inn = dto.Bin;
                                    legalPerson.Within<KazakhstanLegalPersonPart>().SetPropertyValue(part => part.IdentityCardNumber, dto.IdentityCardNumber);
                                    legalPerson.Within<KazakhstanLegalPersonPart>().SetPropertyValue(part => part.IdentityCardIssuedBy, dto.IdentityCardIssuedBy);
                                    legalPerson.Within<KazakhstanLegalPersonPart>().SetPropertyValue(part => part.IdentityCardIssuedOn, dto.IdentityCardIssuedOn);
                                    legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
                                    legalPerson.ClientId = dto.ClientRef.Id;
                                    legalPerson.Comment = dto.Comment;
                                    legalPerson.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}