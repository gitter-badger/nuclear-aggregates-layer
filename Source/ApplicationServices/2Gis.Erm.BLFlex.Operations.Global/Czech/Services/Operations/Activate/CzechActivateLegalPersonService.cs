using System.Linq;

using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.API.Operations.Generic.Activate;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Services.Operations.Activate
{
    public sealed class CzechActivateLegalPersonService : IActivateGenericEntityService<LegalPerson>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CzechActivateLegalPersonService(
            IFinder finder,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
        }

        public int Activate(long entityId)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity, LegalPerson>())
            {
                var restoringLegalPerson = _finder.Find(GenericSpecifications.ById<LegalPerson>(entityId)).Single();

                if (restoringLegalPerson.IsActive)
                {
                    throw new NotificationException(string.Format(BLResources.LegalPersonToRestoreIsAlreadyActive, restoringLegalPerson.LegalName));
                }

                LegalPerson dublicateLegalPerson = null;

                switch ((LegalPersonType)restoringLegalPerson.LegalPersonTypeEnum)
                {
                    case LegalPersonType.LegalPerson:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.Inn))
                        {
                            dublicateLegalPerson = _finder.Find(LegalPersonSpecifications.Find.ActiveLegalPersonsByInn(
                                restoringLegalPerson.Inn))
                                .FirstOrDefault();
                        }

                        break;
                    case LegalPersonType.Businessman:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.Inn))
                        {
                            dublicateLegalPerson = _finder.Find(LegalPersonSpecifications.Find.ActiveBusinessmenByInn(restoringLegalPerson.Inn))
                                .FirstOrDefault();
                        }

                        break;
                    case LegalPersonType.NaturalPerson:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.PassportNumber))
                        {
                            dublicateLegalPerson = _finder.Find(LegalPersonSpecifications.Find.ActiveNaturalPersonsByPassport(
                                restoringLegalPerson.PassportSeries,
                                restoringLegalPerson.PassportNumber))
                                .FirstOrDefault();
                        }

                        break;
                    default:
                        throw new BusinessLogicException(BLResources.UnknownLegalPersonType);
                }

                if (dublicateLegalPerson != null)
                {
                    throw new NotificationException(string.Format(BLResources.ActivateLegalPersonError, dublicateLegalPerson.LegalName));
                }

                var result = _legalPersonRepository.Activate(entityId);

                operationScope
                    .Updated<LegalPerson>(entityId)
                    .Complete();

                return result;
            }
        }
    }
}