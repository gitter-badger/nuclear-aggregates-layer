using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify
{
    public static class LegalPersonFlexSpecs
    {
        public static class LegalPersons
        {
            public static class Emirates
            {
                public static class Project
                {
                    public static IProjectSpecification<LegalPerson, EmiratesLegalPersonDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<LegalPerson, EmiratesLegalPersonDomainEntityDto>(
                            x =>
                                {
                                    if (x.IsNew())
                                    {
                                        return new EmiratesLegalPersonDomainEntityDto
                                            {
                                                ClientRef = x.ClientId != 0
                                                                ? new EntityReference { Id = x.ClientId, Name = null }
                                                                : new EntityReference(),
                                            };
                                    }

                                    var dto = new EmiratesLegalPersonDomainEntityDto
                                        {
                                            Id = x.Id,
                                            LegalName = x.LegalName,
                                            LegalAddress = x.LegalAddress,
                                            ClientRef = new EntityReference { Id = x.ClientId, Name = null },
                                            Comment = x.Comment,
                                            OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn,
                                            Timestamp = x.Timestamp,
                                            LegalPersonTypeEnum = LegalPersonType.LegalPerson,
                                            CommercialLicense = x.Inn,
                                            CommercialLicenseBeginDate =
                                                x.Within<EmiratesLegalPersonPart>().GetPropertyValue(part => part.CommercialLicenseBeginDate),
                                            CommercialLicenseEndDate =
                                                x.Within<EmiratesLegalPersonPart>().GetPropertyValue(part => part.CommercialLicenseEndDate),
                                        };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<EmiratesLegalPersonDomainEntityDto, LegalPerson> Entity()
                    {
                        return new AssignSpecification<EmiratesLegalPersonDomainEntityDto, LegalPerson>(
                            (dto, legalPerson) =>
                                {
                                    legalPerson.LegalPersonTypeEnum = dto.LegalPersonTypeEnum;
                                    legalPerson.LegalName = dto.LegalName;
                                    legalPerson.LegalAddress = dto.LegalAddress;
                                    legalPerson.Inn = dto.CommercialLicense;
                                    legalPerson.ClientId = dto.ClientRef.Id;
                                    legalPerson.Comment = dto.Comment;
                                    legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
                                    legalPerson.Timestamp = dto.Timestamp;

                                    legalPerson.Within<EmiratesLegalPersonPart>()
                                               .SetPropertyValue(part => part.CommercialLicenseBeginDate, dto.CommercialLicenseBeginDate.Value);
                                    legalPerson.Within<EmiratesLegalPersonPart>()
                                               .SetPropertyValue(part => part.CommercialLicenseEndDate, dto.CommercialLicenseEndDate.Value);
                                });
                    }
                }
            }
        }

        public static class LegalPersonProfiles
        {
            public static class Emirates
            {
                public static class Project
                {
                    public static IProjectSpecification<LegalPersonProfile, EmiratesLegalPersonProfileDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<LegalPersonProfile, EmiratesLegalPersonProfileDomainEntityDto>(
                            x =>
                                {
                                    if (x.IsNew())
                                    {
                                        return new EmiratesLegalPersonProfileDomainEntityDto
                                            {
                                                LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                                DocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined,
                                                PaymentMethod = PaymentMethod.BankTransaction
                                            };
                                    }

                                    var resultDto = new EmiratesLegalPersonProfileDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            Email = x.Email,
                                            ChiefNameInNominative = x.ChiefNameInNominative,
                                            DocumentsDeliveryAddress = x.DocumentsDeliveryAddress,
                                            DocumentsDeliveryMethod = x.DocumentsDeliveryMethod,
                                            LegalPersonRef = new EntityReference { Id = x.LegalPersonId },
                                            PositionInNominative = x.PositionInNominative,
                                            PostAddress = x.PostAddress,
                                            EmailForAccountingDocuments = x.EmailForAccountingDocuments,
                                            PaymentEssentialElements = x.PaymentEssentialElements,
                                            PaymentMethod = x.PaymentMethod == null
                                                                ? PaymentMethod.Undefined
                                                                : (PaymentMethod)x.PaymentMethod,
                                            IBAN = x.IBAN,
                                            SWIFT = x.SWIFT,
                                            BankName = x.BankName,
                                            PersonResponsibleForDocuments = x.PersonResponsibleForDocuments,
                                            RecipientName = x.RecipientName,
                                            IsMainProfile = x.IsMainProfile,
                                            OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn,
                                            Timestamp = x.Timestamp,
                                            Phone = x.Within<EmiratesLegalPersonProfilePart>().GetPropertyValue(part => part.Phone),
                                            Fax = x.Within<EmiratesLegalPersonProfilePart>().GetPropertyValue(part => part.Fax),
                                        };

                                    return resultDto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<EmiratesLegalPersonProfileDomainEntityDto, LegalPersonProfile> Entity()
                    {
                        return new AssignSpecification<EmiratesLegalPersonProfileDomainEntityDto, LegalPersonProfile>(
                            (dto, legalPersonProfile) =>
                                {
                                    legalPersonProfile.Name = dto.Name;
                                    legalPersonProfile.LegalPersonId = dto.LegalPersonRef.Id.Value;
                                    legalPersonProfile.DocumentsDeliveryAddress = dto.DocumentsDeliveryAddress;
                                    legalPersonProfile.RecipientName = dto.RecipientName;
                                    legalPersonProfile.PersonResponsibleForDocuments = dto.PersonResponsibleForDocuments;
                                    legalPersonProfile.DocumentsDeliveryMethod = dto.DocumentsDeliveryMethod;
                                    legalPersonProfile.EmailForAccountingDocuments = dto.EmailForAccountingDocuments;
                                    legalPersonProfile.Email = dto.Email;
                                    legalPersonProfile.PostAddress = dto.PostAddress;
                                    legalPersonProfile.PaymentMethod = dto.PaymentMethod;
                                    legalPersonProfile.IBAN = dto.IBAN;
                                    legalPersonProfile.SWIFT = dto.SWIFT;
                                    legalPersonProfile.BankName = dto.BankName;
                                    legalPersonProfile.PaymentEssentialElements = dto.PaymentEssentialElements;
                                    legalPersonProfile.ChiefNameInNominative = dto.ChiefNameInNominative;
                                    legalPersonProfile.PositionInNominative = dto.PositionInNominative;
                                    legalPersonProfile.OwnerCode = dto.OwnerRef.Id.Value;

                                    legalPersonProfile.Timestamp = dto.Timestamp;

                                    legalPersonProfile.Within<EmiratesLegalPersonProfilePart>()
                                                      .SetPropertyValue(part => part.Fax, dto.Fax);
                                    legalPersonProfile.Within<EmiratesLegalPersonProfilePart>()
                                                      .SetPropertyValue(part => part.Phone, dto.Phone);
                                });
                    }
                }
            }
        }
    }
}