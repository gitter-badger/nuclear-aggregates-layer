using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.LegalPersons
{
    public sealed class ChileValidatePaymentRequisitesIsUniqueHandler : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, IChileAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;

        public ChileValidatePaymentRequisitesIsUniqueHandler(
            ILegalPersonRepository legalPersonRepository)
        {
            _legalPersonRepository = legalPersonRepository;
        }

        protected override EmptyResponse Handle(ValidatePaymentRequisitesIsUniqueRequest request)
        {
            var modelLegalPersonType = request.Entity.LegalPersonTypeEnum;
            var rut = !string.IsNullOrEmpty(request.Entity.Inn) ? request.Entity.Inn.Trim() : null;

            var rutDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, rut);
            if (rutDublicate.ActiveDublicateExists)
            {
                throw new NotificationException(GetActiveDublicateMessage(modelLegalPersonType));
            }

            if (rutDublicate.InactiveDublicateExists)
            {
                throw new NotificationException(GetIncativeDublicateMessage(modelLegalPersonType));
            }

            if (rutDublicate.DeletedDublicateExists)
            {
                throw new NotificationException(GetDeletedDublicateMessage(modelLegalPersonType));
            }

            return Response.Empty;
        }

        private static string GetActiveDublicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.ChileActiveLegalPersonWithSpecifiedRutAlreadyExist
                : BLResources.ChileActiveBusinessmanWithSpecifiedRutAlreadyExist;
        }

        private static string GetIncativeDublicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.ChileInactiveLegalPersonWithSpecifiedRutAlreadyExist
                : BLResources.ChileInactiveBusinessmanWithSpecifiedRutAlreadyExist;
        }

        private static string GetDeletedDublicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.ChileDeletedLegalPersonWithSpecifiedRutAlreadyExist
                : BLResources.ChileDeletedBusinessmanWithSpecifiedRutAlreadyExist;
        }
    }
}