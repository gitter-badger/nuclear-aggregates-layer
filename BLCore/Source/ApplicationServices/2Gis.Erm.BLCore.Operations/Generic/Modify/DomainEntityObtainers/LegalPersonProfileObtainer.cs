using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    // TODO {all, 07.04.2014}: в  целом по obtainers см. коммент к IBusinessModelEntityObtainerFlex, до выработки болееменее четкой идеологии дальнейшего развития EAV и т.п., предлагаю пока дальше obtainers такого типа не масштабировать/клонировать  
    public sealed class LegalPersonProfileObtainer : IBusinessModelEntityObtainer<LegalPersonProfile>, IAggregateReadModel<LegalPerson>
    {
        private readonly IFinder _finder;

        public LegalPersonProfileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPersonProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LegalPersonProfileDomainEntityDto)domainEntityDto;

            var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(dto.Id)).One()
                ?? new LegalPersonProfile { IsActive = true };

            legalPersonProfile.Name = dto.Name;
            legalPersonProfile.PositionInGenitive = dto.PositionInGenitive;
            legalPersonProfile.PositionInNominative = dto.PositionInNominative;
            legalPersonProfile.Registered = dto.Registered;
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
            legalPersonProfile.BankCode = dto.BankCode;
            legalPersonProfile.BankName = dto.BankName;
            legalPersonProfile.BankAddress = dto.BankAddress;
            legalPersonProfile.IBAN = dto.IBAN;
            legalPersonProfile.SWIFT = dto.SWIFT;
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

            return legalPersonProfile;
        }
    }
}