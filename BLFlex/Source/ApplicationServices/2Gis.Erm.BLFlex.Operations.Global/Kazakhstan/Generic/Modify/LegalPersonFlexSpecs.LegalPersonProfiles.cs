using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify
{
    public partial class LegalPersonFlexSpecs
    {
        public static class LegalPersonProfiles
        {
            public static class Kazakhstan
            {
                public static class Project
                {
                    public static IProjectSpecification<LegalPersonProfile, KazakhstanLegalPersonProfileDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<LegalPersonProfile, KazakhstanLegalPersonProfileDomainEntityDto>(
                            x =>
                                {
                                    if (x.IsNew())
                                    {
                                        return new KazakhstanLegalPersonProfileDomainEntityDto
                                                   {
                                                       LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                                       PaymentMethod = PaymentMethod.BankTransaction,
                                                       DocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined
                                                   };
                                    }

                                    var dto = new KazakhstanLegalPersonProfileDomainEntityDto
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
                                                      PostAddress = x.PostAddress,
                                                      ActualAddress = x.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(part => part.ActualAddress),
                                                      EmailForAccountingDocuments = x.EmailForAccountingDocuments,
                                                      PaymentMethod = x.PaymentMethod == null
                                                                          ? PaymentMethod.Undefined
                                                                          : (PaymentMethod)x.PaymentMethod,
                                                      PersonResponsibleForDocuments = x.PersonResponsibleForDocuments,
                                                      Phone = x.Phone,
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
                                                      PaymentEssentialElements = x.PaymentEssentialElements,
                                                      BankName = x.BankName,
                                                      IBAN = x.IBAN,
                                                      SWIFT = x.SWIFT,

                                                      OperatesOnTheBasisInGenitive = x.OperatesOnTheBasisInGenitive == null
                                                                                         ? OperatesOnTheBasisType.Undefined
                                                                                         : (OperatesOnTheBasisType)x.OperatesOnTheBasisInGenitive,
                                                      OtherAuthorityDocument = x.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(part => part.OtherAuthorityDocument),

                                                      CertificateDate = x.CertificateDate,
                                                      CertificateNumber = x.CertificateNumber,

                                                      WarrantyNumber = x.WarrantyNumber,
                                                      WarrantyBeginDate = x.WarrantyBeginDate,
                                                      WarrantyEndDate = x.WarrantyEndDate,

                                                      DecreeDate = x.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(part => part.DecreeDate),
                                                      DecreeNumber = x.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(part => part.DecreeNumber)
                                                  };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<KazakhstanLegalPersonProfileDomainEntityDto, LegalPersonProfile> Entity()
                    {
                        return new AssignSpecification<KazakhstanLegalPersonProfileDomainEntityDto, LegalPersonProfile>(
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
                                    legalPersonProfile.Within<KazakhstanLegalPersonProfilePart>().SetPropertyValue(part => part.ActualAddress, dto.ActualAddress);
                                    legalPersonProfile.RecipientName = dto.RecipientName;
                                    legalPersonProfile.DocumentsDeliveryMethod = dto.DocumentsDeliveryMethod;
                                    legalPersonProfile.EmailForAccountingDocuments = dto.EmailForAccountingDocuments;
                                    legalPersonProfile.Email = dto.Email;
                                    legalPersonProfile.PersonResponsibleForDocuments = dto.PersonResponsibleForDocuments;
                                    legalPersonProfile.Phone = dto.Phone;
                                    legalPersonProfile.OwnerCode = dto.OwnerRef.Id.Value;
                                    legalPersonProfile.PaymentMethod = dto.PaymentMethod;
                                    legalPersonProfile.LegalPersonId = dto.LegalPersonRef.Id.Value;
                                    legalPersonProfile.CertificateDate = dto.CertificateDate;
                                    legalPersonProfile.CertificateNumber = dto.CertificateNumber;
                                    legalPersonProfile.WarrantyBeginDate = dto.WarrantyBeginDate;
                                    legalPersonProfile.WarrantyEndDate = dto.WarrantyEndDate;
                                    legalPersonProfile.WarrantyNumber = dto.WarrantyNumber;
                                    legalPersonProfile.Timestamp = dto.Timestamp;

                                    legalPersonProfile.BankName = dto.BankName;
                                    legalPersonProfile.PaymentEssentialElements = dto.PaymentEssentialElements;
                                    legalPersonProfile.IBAN = dto.IBAN;
                                    legalPersonProfile.SWIFT = dto.SWIFT;

                                    legalPersonProfile.Within<KazakhstanLegalPersonProfilePart>().SetPropertyValue(part => part.OtherAuthorityDocument, dto.OtherAuthorityDocument);

                                    legalPersonProfile.CertificateDate = dto.CertificateDate;
                                    legalPersonProfile.CertificateNumber = dto.CertificateNumber;

                                    legalPersonProfile.WarrantyNumber = dto.WarrantyNumber;
                                    legalPersonProfile.WarrantyBeginDate = dto.WarrantyBeginDate;
                                    legalPersonProfile.WarrantyEndDate = dto.WarrantyEndDate;

                                    legalPersonProfile.Within<KazakhstanLegalPersonProfilePart>().SetPropertyValue(part => part.DecreeDate, dto.DecreeDate);
                                    legalPersonProfile.Within<KazakhstanLegalPersonProfilePart>().SetPropertyValue(part => part.DecreeNumber, dto.DecreeNumber);
                                });
                    }
                }
            }
        }
    }
}