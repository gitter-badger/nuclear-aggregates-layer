using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Kazakhstan;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.LegalPersons
{
    public class KazakhstanValidatePaymentRequisitesIsUnique : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, IKazakhstanAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IKazakhstanLegalPersonReadModel _flexReadModel;

        public KazakhstanValidatePaymentRequisitesIsUnique(ILegalPersonRepository legalPersonRepository, IKazakhstanLegalPersonReadModel flexReadModel)
        {
            _legalPersonRepository = legalPersonRepository;
            _flexReadModel = flexReadModel;
        }

        protected override EmptyResponse Handle(ValidatePaymentRequisitesIsUniqueRequest request)
        {
            var legalPerson = request.Entity;

            var iinOrBin = legalPerson.Inn;
            var iinDuplicate = _legalPersonRepository.CheckIfExistsInnDuplicate(legalPerson.Id, iinOrBin);
            CheckResult(iinDuplicate,
                        BLResources.ActiveLegalPersonWithSpecifiedIinExists,
                        BLResources.InactiveLegalPersonWithSpecifiedIinExists,
                        BLResources.DeletedLegalPersonWithSpecifiedIinExists);


            if((LegalPersonType)legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson)
            {
                var idCardNumber = legalPerson.Within<KazakhstanLegalPersonPart>().GetPropertyValue(x => x.IdentityCardNumber);
                var idCardDuplicate = _flexReadModel.CheckIfExistsIdentityCardDuplicate(legalPerson.Id, idCardNumber);
                CheckResult(idCardDuplicate,
                            BLResources.ActiveLegalPersonWithSpecifiedIdentityCardNumberExists,
                            BLResources.InactiveLegalPersonWithSpecifiedIdentityCardNumberExists,
                            BLResources.InactiveLegalPersonWithSpecifiedIdentityCardNumberExists);
            }

            return Response.Empty;
        }

        private static void CheckResult(CheckForDublicatesResultDto result,
                                        string activeDublicatesMessage,
                                        string inactiveDublicatesMessage,
                                        string deletedDublicatesMessage)
        {
            if (result.ActiveDublicateExists)
            {
                throw new NotificationException(activeDublicatesMessage);
            }

            if (result.InactiveDublicateExists)
            {
                throw new NotificationException(inactiveDublicatesMessage);
            }

            if (result.DeletedDublicateExists)
            {
                throw new NotificationException(deletedDublicatesMessage);
            }
        }
    }
}