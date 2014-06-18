using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.LegalPersons
{
    public sealed class EmiratesValidatePaymentRequisitesIsUniqueHandler : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, IEmiratesAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;

        public EmiratesValidatePaymentRequisitesIsUniqueHandler(
            ILegalPersonRepository legalPersonRepository)
        {
            _legalPersonRepository = legalPersonRepository;
        }

        protected override EmptyResponse Handle(ValidatePaymentRequisitesIsUniqueRequest request)
        {
            var commercialLicense = request.Entity.Inn.Trim();

            var commercialLicenseDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, commercialLicense);
            if (commercialLicenseDublicate.ActiveDublicateExists)
            {
                throw new NotificationException(BLResources.EmiratesActiveLegalPersonWithSpecifiedCommercialLicenseAlreadyExist);
            }

            if (commercialLicenseDublicate.InactiveDublicateExists)
            {
                throw new NotificationException(BLResources.EmiratesInactiveLegalPersonWithSpecifiedCommercialLicenseAlreadyExist);
            }

            if (commercialLicenseDublicate.DeletedDublicateExists)
            {
                throw new NotificationException(BLResources.EmiratesDeletedLegalPersonWithSpecifiedCommercialLicenseAlreadyExist);
            }

            return Response.Empty;
        }
    }
}