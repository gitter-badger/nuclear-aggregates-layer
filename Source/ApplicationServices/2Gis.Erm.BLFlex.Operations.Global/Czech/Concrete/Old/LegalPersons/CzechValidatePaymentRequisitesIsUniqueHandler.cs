using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.LegalPersons
{
    public sealed class CzechValidatePaymentRequisitesIsUniqueHandler : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, ICzechAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;

        public CzechValidatePaymentRequisitesIsUniqueHandler(
            ILegalPersonRepository legalPersonRepository)
        {
            _legalPersonRepository = legalPersonRepository;
        }

        protected override EmptyResponse Handle(ValidatePaymentRequisitesIsUniqueRequest request)
        {
            var modelLegalPersonType = (LegalPersonType)request.Entity.LegalPersonTypeEnum;
            var dic = !string.IsNullOrEmpty(request.Entity.Inn) ? request.Entity.Inn.Trim() : null;
            var ic = !string.IsNullOrEmpty(request.Entity.Ic) ? request.Entity.Ic.Trim() : null;

            var dicAndIcDublicate = _legalPersonRepository.CheckIfExistsInnOrIcDuplicate(request.Entity.Id, dic, ic);
            if (dicAndIcDublicate.ActiveDublicateExists)
            {
                throw new NotificationException(GetActiveDublicateMessage(modelLegalPersonType));
            }

            if (dicAndIcDublicate.InactiveDublicateExists)
            {
                throw new NotificationException(GetIncativeDublicateMessage(modelLegalPersonType));
            }

            if (dicAndIcDublicate.DeletedDublicateExists)
            {
                throw new NotificationException(GetDeletedDublicateMessage(modelLegalPersonType));
            }

            return Response.Empty;
        }

        private static string GetActiveDublicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.CzechActiveLegalPersonWithSpecifiedDicAndIcAlreadyExist
                : BLResources.CzechActiveBusinessmanWithSpecifiedDicAndIcAlreadyExist;
        }

        private static string GetIncativeDublicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.CzechInactiveLegalPersonWithSpecifiedDicAndIcAlreadyExist
                : BLResources.CzechInactiveBusinessmanWithSpecifiedDicAndIcAlreadyExist;
        }

        private static string GetDeletedDublicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                ? BLResources.CzechDeletedLegalPersonWithSpecifiedDicAndIcAlreadyExist
                : BLResources.CzechDeletedBusinessmanWithSpecifiedDicAndIcAlreadyExist;
        }
    }
}