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
        public static class LegalPersonProfiles
        {
            public static class Chile
            {
                public static class Project
                {
                    public static MapSpecification<LegalPersonProfile, ChileLegalPersonProfileDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<LegalPersonProfile, ChileLegalPersonProfileDomainEntityDto>(
                            x =>
                            {
                                if (x.IsNew())
                                {
                                    return new ChileLegalPersonProfileDomainEntityDto
                                        {
                                            LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                            BankRef = new EntityReference(),
                                            PaymentMethod = PaymentMethod.BankTransaction,
                                            DocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined,
                                        };
                                }

                                var bankId = x.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.BankId);
                                var dto = new ChileLegalPersonProfileDomainEntityDto
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Email = x.Email,
                                        ChiefNameInGenitive = x.ChiefNameInGenitive,
                                        ChiefNameInNominative = x.ChiefNameInNominative,
                                        DocumentsDeliveryAddress = x.DocumentsDeliveryAddress,
                                        DocumentsDeliveryMethod = x.DocumentsDeliveryMethod,
                                        LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                        PositionInNominative = x.PositionInNominative,
                                        PositionInGenitive = x.PositionInGenitive,
                                        OperatesOnTheBasisInGenitive = x.OperatesOnTheBasisInGenitive ?? OperatesOnTheBasisType.Undefined,
                                        PostAddress = x.PostAddress,
                                        EmailForAccountingDocuments = x.EmailForAccountingDocuments,
                                        PaymentEssentialElements = x.PaymentEssentialElements,
                                        PaymentMethod = x.PaymentMethod == null ? PaymentMethod.Undefined : (PaymentMethod)x.PaymentMethod,
                                        AccountNumber = x.AccountNumber,
                                        PersonResponsibleForDocuments = x.PersonResponsibleForDocuments,
                                        Phone = x.Phone,
                                        RecipientName = x.RecipientName,
                                        IsMainProfile = x.IsMainProfile,
                                        AccountType = x.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.AccountType),
                                        BankRef = new EntityReference { Id = bankId, Name = null },
                                        RepresentativeRut = x.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.RepresentativeRut),
                                        RepresentativeDocumentIssuedOn = x.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.RepresentativeAuthorityDocumentIssuedOn),
                                        RepresentativeDocumentIssuedBy = x.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.RepresentativeAuthorityDocumentIssuedBy),
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
                    public static IAssignSpecification<ChileLegalPersonProfileDomainEntityDto, LegalPersonProfile> Entity()
                    {
                        return new AssignSpecification<ChileLegalPersonProfileDomainEntityDto, LegalPersonProfile>(
                            (dto, legalPersonProfile) =>
                            {
                                legalPersonProfile.Name = dto.Name;
                                legalPersonProfile.PositionInGenitive = dto.PositionInGenitive;
                                legalPersonProfile.PositionInNominative = dto.PositionInNominative;
                                legalPersonProfile.ChiefNameInNominative = dto.ChiefNameInNominative;
                                legalPersonProfile.ChiefNameInGenitive = dto.ChiefNameInGenitive;
                                legalPersonProfile.OperatesOnTheBasisInGenitive = dto.OperatesOnTheBasisInGenitive;
                                legalPersonProfile.DocumentsDeliveryAddress = dto.DocumentsDeliveryAddress;
                                legalPersonProfile.PostAddress = dto.PostAddress;
                                legalPersonProfile.RecipientName = dto.RecipientName;
                                legalPersonProfile.DocumentsDeliveryMethod = dto.DocumentsDeliveryMethod;
                                legalPersonProfile.EmailForAccountingDocuments = dto.EmailForAccountingDocuments;
                                legalPersonProfile.Email = dto.Email;
                                legalPersonProfile.PersonResponsibleForDocuments = dto.PersonResponsibleForDocuments;
                                legalPersonProfile.Phone = dto.Phone;
                                legalPersonProfile.OwnerCode = dto.OwnerRef.Id.Value;
                                legalPersonProfile.AccountNumber = dto.AccountNumber;
                                legalPersonProfile.PaymentEssentialElements = dto.PaymentEssentialElements;
                                legalPersonProfile.PaymentMethod = dto.PaymentMethod;
                                legalPersonProfile.LegalPersonId = dto.LegalPersonRef.Id.Value;
                                legalPersonProfile.Within<ChileLegalPersonProfilePart>().SetPropertyValue(part => part.AccountType, dto.AccountType);
                                legalPersonProfile.Within<ChileLegalPersonProfilePart>().SetPropertyValue(part => part.BankId, dto.BankRef.Id);
                                legalPersonProfile.Within<ChileLegalPersonProfilePart>().SetPropertyValue(part => part.RepresentativeRut, dto.RepresentativeRut);
                                legalPersonProfile.Within<ChileLegalPersonProfilePart>().SetPropertyValue(part => part.RepresentativeAuthorityDocumentIssuedBy, dto.RepresentativeDocumentIssuedBy);
                                legalPersonProfile.Within<ChileLegalPersonProfilePart>().SetPropertyValue(part => part.RepresentativeAuthorityDocumentIssuedOn, dto.RepresentativeDocumentIssuedOn);
                                legalPersonProfile.Timestamp = dto.Timestamp;
                            });
                    }
                }
            }
        }
    }
}