using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
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

            var legalPersonProfile =
                dto.Id == 0
                    ? new LegalPersonProfile { IsActive = true }
                    : _finder.Find(Specs.Find.ById<LegalPersonProfile>(dto.Id)).Single();

            legalPersonProfile.Name = dto.Name.EnsureСleanness();
            legalPersonProfile.PositionInGenitive = dto.PositionInGenitive;
            legalPersonProfile.PositionInNominative = dto.PositionInNominative.EnsureСleanness();
            legalPersonProfile.Registered = dto.Registered.EnsureСleanness();
            legalPersonProfile.ChiefNameInNominative = dto.ChiefNameInNominative.EnsureСleanness();
            legalPersonProfile.ChiefNameInGenitive = dto.ChiefNameInGenitive.EnsureСleanness();
            legalPersonProfile.OperatesOnTheBasisInGenitive = (int)dto.OperatesOnTheBasisInGenitive;
            legalPersonProfile.DocumentsDeliveryAddress = dto.DocumentsDeliveryAddress.EnsureСleanness();
            legalPersonProfile.PostAddress = dto.PostAddress.EnsureСleanness();
            legalPersonProfile.RecipientName = dto.RecipientName.EnsureСleanness();
            legalPersonProfile.DocumentsDeliveryMethod = (int)dto.DocumentsDeliveryMethod;
            legalPersonProfile.EmailForAccountingDocuments = dto.EmailForAccountingDocuments.EnsureСleanness();
            legalPersonProfile.AdditionalEmail = dto.AdditionalEmail.EnsureСleanness();
            legalPersonProfile.PersonResponsibleForDocuments = dto.PersonResponsibleForDocuments.EnsureСleanness();
            legalPersonProfile.Phone = dto.Phone.EnsureСleanness();
            legalPersonProfile.OwnerCode = dto.OwnerRef.Id.Value;
            legalPersonProfile.PaymentEssentialElements = dto.PaymentEssentialElements.EnsureСleanness();
            legalPersonProfile.AccountNumber = dto.AccountNumber.EnsureСleanness();
            legalPersonProfile.BankCode = dto.BankCode.EnsureСleanness();
            legalPersonProfile.BankName = dto.BankName.EnsureСleanness();
            legalPersonProfile.BankAddress = dto.BankAddress.EnsureСleanness();
            legalPersonProfile.IBAN = dto.IBAN.EnsureСleanness();
            legalPersonProfile.SWIFT = dto.SWIFT.EnsureСleanness();
            legalPersonProfile.AdditionalPaymentElements = dto.AdditionalPaymentElements.EnsureСleanness();
            legalPersonProfile.PaymentMethod = (int?)dto.PaymentMethod;
            legalPersonProfile.LegalPersonId = dto.LegalPersonRef.Id.Value;
            legalPersonProfile.CertificateDate = dto.CertificateDate;
            legalPersonProfile.CertificateNumber = dto.CertificateNumber.EnsureСleanness();
            legalPersonProfile.WarrantyBeginDate = dto.WarrantyBeginDate;
            legalPersonProfile.WarrantyEndDate = dto.WarrantyEndDate;
            legalPersonProfile.WarrantyNumber = dto.WarrantyNumber.EnsureСleanness();
            legalPersonProfile.RegistrationCertificateDate = dto.RegistrationCertificateDate;
            legalPersonProfile.RegistrationCertificateNumber = dto.RegistrationCertificateNumber.EnsureСleanness();
            legalPersonProfile.BargainBeginDate = dto.BargainBeginDate;
            legalPersonProfile.BargainEndDate = dto.BargainEndDate;
            legalPersonProfile.BargainNumber = dto.BargainNumber.EnsureСleanness();
            legalPersonProfile.Timestamp = dto.Timestamp;


            return legalPersonProfile;
        }
    }
}