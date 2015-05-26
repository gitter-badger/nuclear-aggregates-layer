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
        public static class LegalPersonProfiles
        {
            public static class Ukraine
            {
                public static class Project
                {
                    public static ProjectSpecification<LegalPersonProfile, UkraineLegalPersonProfileDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<LegalPersonProfile, UkraineLegalPersonProfileDomainEntityDto>(
                            x =>
                                {
                                    if (x.IsNew())
                                    {
                                        return new UkraineLegalPersonProfileDomainEntityDto
                                            {
                                                LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                                PaymentMethod = PaymentMethod.BankTransaction,
                                                DocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined
                                            };
                                    }

                                    var dto = new UkraineLegalPersonProfileDomainEntityDto
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
                                            CertificateDate = x.CertificateDate,
                                            CertificateNumber = x.CertificateNumber,
                                            WarrantyNumber = x.WarrantyNumber,
                                            WarrantyBeginDate = x.WarrantyBeginDate,
                                            WarrantyEndDate = x.WarrantyEndDate,
                                            PostAddress = x.PostAddress,
                                            EmailForAccountingDocuments = x.EmailForAccountingDocuments,
                                            PaymentEssentialElements = x.PaymentEssentialElements,
                                            PaymentMethod = x.PaymentMethod == null
                                                                ? PaymentMethod.Undefined
                                                                : (PaymentMethod)x.PaymentMethod,
                                            AccountNumber = x.AccountNumber,
                                            BankName = x.BankName,
                                            PersonResponsibleForDocuments = x.PersonResponsibleForDocuments,
                                            Phone = x.Phone,
                                            RecipientName = x.RecipientName,
                                            IsMainProfile = x.IsMainProfile,
                                            Mfo = x.Within<UkraineLegalPersonProfilePart>().GetPropertyValue(part => part.Mfo),
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
                    public static IAssignSpecification<UkraineLegalPersonProfileDomainEntityDto, LegalPersonProfile> Entity()
                    {
                        return new AssignSpecification<UkraineLegalPersonProfileDomainEntityDto, LegalPersonProfile>(
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
                                    legalPersonProfile.PaymentEssentialElements = dto.PaymentEssentialElements;
                                    legalPersonProfile.AccountNumber = dto.AccountNumber;
                                    legalPersonProfile.BankName = dto.BankName;
                                    legalPersonProfile.PaymentMethod = dto.PaymentMethod;
                                    legalPersonProfile.LegalPersonId = dto.LegalPersonRef.Id.Value;
                                    legalPersonProfile.CertificateDate = dto.CertificateDate;
                                    legalPersonProfile.CertificateNumber = dto.CertificateNumber;
                                    legalPersonProfile.WarrantyBeginDate = dto.WarrantyBeginDate;
                                    legalPersonProfile.WarrantyEndDate = dto.WarrantyEndDate;
                                    legalPersonProfile.WarrantyNumber = dto.WarrantyNumber;
                                    legalPersonProfile.Within<UkraineLegalPersonProfilePart>().SetPropertyValue(part => part.Mfo, dto.Mfo);
                                    legalPersonProfile.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}