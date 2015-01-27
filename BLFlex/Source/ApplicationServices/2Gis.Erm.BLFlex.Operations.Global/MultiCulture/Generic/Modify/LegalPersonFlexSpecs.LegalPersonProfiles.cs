using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify
{
    public partial class LegalPersonFlexSpecs
    {
        public static class LegalPersonProfiles
        {
            public static class MultiCulture
            {
                public static class Project
                {
                    public static IProjectSpecification<LegalPersonProfile, LegalPersonProfileDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<LegalPersonProfile, LegalPersonProfileDomainEntityDto>(
                            x =>
                                {
                                    if (x.IsNew())
                                    {
                                        return new LegalPersonProfileDomainEntityDto
                                        {
                                            LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                            PaymentMethod = PaymentMethod.BankTransaction,
                                            DocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined,
                                        };
                                    }

                                    var dto = new LegalPersonProfileDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            AdditionalEmail = x.AdditionalEmail,
                                            ChiefNameInGenitive = x.ChiefNameInGenitive,
                                            ChiefNameInNominative = x.ChiefNameInNominative,
                                            Registered = x.Registered,
                                            DocumentsDeliveryAddress = x.DocumentsDeliveryAddress,
                                            DocumentsDeliveryMethod = x.DocumentsDeliveryMethod,
                                            LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                            PositionInNominative = x.PositionInNominative,
                                            PositionInGenitive = x.PositionInGenitive,
                                            OperatesOnTheBasisInGenitive = x.OperatesOnTheBasisInGenitive ?? OperatesOnTheBasisType.Undefined,
                                            CertificateDate = x.CertificateDate,
                                            CertificateNumber = x.CertificateNumber,
                                            BargainBeginDate = x.BargainBeginDate,
                                            BargainEndDate = x.BargainEndDate,
                                            BargainNumber = x.BargainNumber,
                                            WarrantyNumber = x.WarrantyNumber,
                                            WarrantyBeginDate = x.WarrantyBeginDate,
                                            WarrantyEndDate = x.WarrantyEndDate,
                                            PostAddress = x.PostAddress,
                                            EmailForAccountingDocuments = x.EmailForAccountingDocuments,
                                            PaymentEssentialElements = x.PaymentEssentialElements,
                                            PaymentMethod = x.PaymentMethod == null
                                                                                      ? PaymentMethod.Undefined
                                                                                      : (PaymentMethod)x.PaymentMethod,
                                            IBAN = x.IBAN,
                                            SWIFT = x.SWIFT,
                                            AccountNumber = x.AccountNumber,
                                            BankCode = x.BankCode,
                                            BankName = x.BankName,
                                            BankAddress = x.BankAddress,
                                            RegistrationCertificateDate = x.RegistrationCertificateDate,
                                            RegistrationCertificateNumber = x.RegistrationCertificateNumber,
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
                                            Timestamp = x.Timestamp
                                        };

                                    return dto;
                                });
                    }
                }

                //                public static class Assign
                //                {
                //                    public static IAssignSpecification<UkraineLegalPersonDomainEntityDto, LegalPerson> Entity()
                //                    {
                //                        return new AssignSpecification<UkraineLegalPersonDomainEntityDto, LegalPerson>(
                //                            (dto, legalPerson) =>
                //                                {
                //                                    legalPerson.LegalPersonTypeEnum = (int)dto.LegalPersonTypeEnum;
                //                                    legalPerson.LegalName = dto.LegalName;
                //                                    legalPerson.ShortName = dto.ShortName;
                //                                    legalPerson.LegalAddress = dto.LegalAddress;
                //                                    legalPerson.PassportNumber = dto.PassportNumber;
                //                                    legalPerson.RegistrationAddress = dto.RegistrationAddress;
                //                                    legalPerson.Inn = dto.Inn;
                //                                    legalPerson.PassportSeries = dto.PassportSeries;
                //                                    legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
                //                                    legalPerson.ClientId = dto.ClientRef.Id;
                //                                    legalPerson.Comment = dto.Comment;
                //                                    legalPerson.PassportIssuedBy = dto.PassportIssuedBy;
                //                                    legalPerson.Timestamp = dto.Timestamp;
                //
                //                                    var part = legalPerson.UkrainePart();
                //                                    if (part != null)
                //                                    {
                //                                        part.Egrpou = dto.Egrpou;
                //                                        part.TaxationType = dto.TaxationType;
                //                                    }
                //                                });
                //                    }
                //                }
            }
        }
    }
}