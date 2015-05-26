using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Russia;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify
{
    public static partial class LegalPersonFlexSpecs
    {
        public static class LegalPersonProfiles
        {
            public static class Russia
            {
                public static class Project
                {
                    public static ProjectSpecification<LegalPersonProfile, RussiaLegalPersonProfileDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<LegalPersonProfile, RussiaLegalPersonProfileDomainEntityDto>(
                            x =>
                            {
                                if (x.IsNew())
                                {
                                    return new RussiaLegalPersonProfileDomainEntityDto
                                        {
                                            LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = null },
                                            DocumentsDeliveryMethod = DocumentsDeliveryMethod.PostOnly,
                                            OperatesOnTheBasisInGenitive = OperatesOnTheBasisType.Undefined,
                                        };
                                }

                                var dto = new RussiaLegalPersonProfileDomainEntityDto
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Email = x.Email,
                                    ChiefNameInGenitive = x.ChiefNameInGenitive,
                                    ChiefNameInNominative = x.ChiefNameInNominative,
                                    ChiefFullNameInNominative = x.Within<RussiaLegalPersonProfilePart>().GetPropertyValue(part => part.ChiefFullNameInNominative),
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

                public static class Assign
                {
                    public static IAssignSpecification<RussiaLegalPersonProfileDomainEntityDto, LegalPersonProfile> Entity()
                    {
                        return new AssignSpecification<RussiaLegalPersonProfileDomainEntityDto, LegalPersonProfile>(
                            (dto, legalPersonProfile) =>
                            {
                                legalPersonProfile.Name = dto.Name;
                                legalPersonProfile.PositionInGenitive = dto.PositionInGenitive;
                                legalPersonProfile.PositionInNominative = dto.PositionInNominative;
                                legalPersonProfile.Registered = dto.Registered;
                                legalPersonProfile.ChiefNameInNominative = dto.ChiefNameInNominative;
                                legalPersonProfile.ChiefNameInGenitive = dto.ChiefNameInGenitive;
                                legalPersonProfile.Within<RussiaLegalPersonProfilePart>().SetPropertyValue(part => part.ChiefFullNameInNominative, dto.ChiefFullNameInNominative);
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
                                legalPersonProfile.BankCode = dto.BankCode;
                                legalPersonProfile.BankName = dto.BankName;
                                legalPersonProfile.BankAddress = dto.BankAddress;
                                legalPersonProfile.PaymentMethod = dto.PaymentMethod;
                                legalPersonProfile.LegalPersonId = dto.LegalPersonRef.Id.Value;
                                legalPersonProfile.CertificateDate = dto.CertificateDate;
                                legalPersonProfile.CertificateNumber = dto.CertificateNumber;
                                legalPersonProfile.WarrantyBeginDate = dto.WarrantyBeginDate;
                                legalPersonProfile.WarrantyEndDate = dto.WarrantyEndDate;
                                legalPersonProfile.WarrantyNumber = dto.WarrantyNumber;
                                legalPersonProfile.RegistrationCertificateDate = dto.RegistrationCertificateDate;
                                legalPersonProfile.RegistrationCertificateNumber = dto.RegistrationCertificateNumber;
                                legalPersonProfile.BargainBeginDate = dto.BargainBeginDate;
                                legalPersonProfile.BargainEndDate = dto.BargainEndDate;
                                legalPersonProfile.BargainNumber = dto.BargainNumber;
                                legalPersonProfile.Timestamp = dto.Timestamp;
                            });
                    }
                }
            }
        }
    }
}