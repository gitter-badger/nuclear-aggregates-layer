using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public sealed class LegalPersonProfileObtainer : IBusinessModelEntityObtainer<LegalPersonProfile>, IAggregateReadModel<LegalPerson>, IChileAdapted
    {
        private readonly IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance> _legalPersonProfilePartPropertiesConverter;
        private readonly IFinder _finder;

        public LegalPersonProfileObtainer(
            IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance> legalPersonProfilePartPropertiesConverter, 
            IFinder finder)
        {
            _legalPersonProfilePartPropertiesConverter = legalPersonProfilePartPropertiesConverter;
            _finder = finder;
        }

        public LegalPersonProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ChileLegalPersonProfileDomainEntityDto)domainEntityDto;

            var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(dto.Id)).SingleOrDefault()
                                     ?? new LegalPersonProfile { IsActive = true };
            
            CopyLegalPersonProfileFields(dto, legalPersonProfile);

            var legalPersonProfilePart = legalPersonProfile.IsNew()
                                             ? new LegalPersonProfilePart()
                                             : _finder.SingleOrDefault(dto.Id, _legalPersonProfilePartPropertiesConverter.ConvertFromDynamicEntityInstance);

            CopyLegalPersonProfilePartFields(dto, legalPersonProfilePart);

            legalPersonProfile.Parts = new[] { legalPersonProfilePart };
            return legalPersonProfile;
        }

        private void CopyLegalPersonProfileFields(ChileLegalPersonProfileDomainEntityDto source, LegalPersonProfile target)
        {
            target.Name = source.Name.EnsureСleanness();
            target.ChiefNameInNominative = source.RepresentativeName.EnsureСleanness();
            target.PositionInNominative = source.RepresentativePosition.EnsureСleanness();
            target.OperatesOnTheBasisInGenitive = (int)source.OperatesOnTheBasisInGenitive;
            target.DocumentsDeliveryAddress = source.DocumentsDeliveryAddress.EnsureСleanness();
            target.PostAddress = source.PostAddress.EnsureСleanness();
            target.RecipientName = source.RecipientName.EnsureСleanness();
            target.DocumentsDeliveryMethod = (int)source.DocumentsDeliveryMethod;
            target.EmailForAccountingDocuments = source.EmailForAccountingDocuments.EnsureСleanness();
            target.AdditionalEmail = source.AdditionalEmail.EnsureСleanness();
            target.PersonResponsibleForDocuments = source.PersonResponsibleForDocuments.EnsureСleanness();
            target.Phone = source.Phone.EnsureСleanness();
            target.OwnerCode = source.OwnerRef.Id.Value;
            target.AccountNumber = source.AccountNumber.EnsureСleanness();
            target.AdditionalPaymentElements = source.AdditionalPaymentElements.EnsureСleanness();
            target.PaymentMethod = (int?)source.PaymentMethod;
            target.LegalPersonId = source.LegalPersonRef.Id.Value;
            target.Timestamp = source.Timestamp;
        }

        private void CopyLegalPersonProfilePartFields(ChileLegalPersonProfileDomainEntityDto source, LegalPersonProfilePart target)
        {
            target.AccountType = source.AccountType;
            target.BankId = source.BankRef.Id;
            target.RepresentativeAuthorityDocumentIssuedBy = source.RepresentativeDocumentIssuedBy;
            target.RepresentativeAuthorityDocumentIssuedOn = source.RepresentativeDocumentIssuedOn;
            target.RepresentativeRut = source.RepresentativeRut;
        }
    }
}