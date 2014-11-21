using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.LegalPersons
{
    public sealed class UkraineValidatePaymentRequisitesIsUniqueHandler : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, IUkraineAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IUkraineLegalPersonReadModel _ukraineLegalPersonReadModel;

        public UkraineValidatePaymentRequisitesIsUniqueHandler(
            IUkraineLegalPersonReadModel ukraineLegalPersonReadModel,
            ILegalPersonRepository legalPersonRepository)
        {
            _legalPersonRepository = legalPersonRepository;
            _ukraineLegalPersonReadModel = ukraineLegalPersonReadModel;
        }

        protected override EmptyResponse Handle(ValidatePaymentRequisitesIsUniqueRequest request)
        {
            var modelLegalPersonType = (LegalPersonType)request.Entity.LegalPersonTypeEnum;
            var ipn = !string.IsNullOrEmpty(request.Entity.Inn) ? request.Entity.Inn.Trim() : null;

            var ipnDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, ipn);
            if (ipnDublicate.ActiveDublicateExists)
            {
                throw new NotificationException(GetActiveIpnDuplicateMessage(modelLegalPersonType));
            }

            if (ipnDublicate.InactiveDublicateExists)
            {
                throw new NotificationException(GetIncativeIpnDuplicateMessage(modelLegalPersonType));
            }

            if (ipnDublicate.DeletedDublicateExists)
            {
                throw new NotificationException(GetDeletedIpnDuplicateMessage(modelLegalPersonType));
            }

            var egrpou = request.Entity.Within<UkraineLegalPersonPart>().GetPropertyValue(x => x.Egrpou).Trim();
            if (_ukraineLegalPersonReadModel.AreThereAnyActiveEgrpouDuplicates(request.Entity.Id, egrpou))
            {
                throw new NotificationException(GetEgrpouDuplicateMessage(modelLegalPersonType));
            }

            return Response.Empty;
        }

        private static string GetEgrpouDuplicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.UkraineLegalPersonWithSpecifiedEgrpouAlreadyExists
                : BLResources.UkraineBusinessmanWithSpecifiedEgrpouAlreadyExists;
        }

        private static string GetActiveIpnDuplicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.UkraineActiveLegalPersonWithSpecifiedIpnAlreadyExist
                : BLResources.UkraineActiveBusinessmanWithSpecifiedIpnAlreadyExist;
        }

        private static string GetIncativeIpnDuplicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.UkraineInactiveLegalPersonWithSpecifiedIpnAlreadyExist
                : BLResources.UkraineInactiveBusinessmanWithSpecifiedIpnAlreadyExist;
        }

        private static string GetDeletedIpnDuplicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.UkraineDeletedLegalPersonWithSpecifiedIpnAlreadyExist
                : BLResources.UkraineDeletedBusinessmanWithSpecifiedIpnAlreadyExist;
        }
    }
}