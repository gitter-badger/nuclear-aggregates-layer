using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Activate
{
    public sealed class UkraineActivateLegalPersonService : IActivateGenericEntityService<LegalPerson>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IUkraineLegalPersonReadModel _ukraineLegalPersonReadModel;

        public UkraineActivateLegalPersonService(
            IFinder finder,
            ILegalPersonReadModel legalPersonReadModel,
            IUkraineLegalPersonReadModel ukraineLegalPersonReadModel,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
            _ukraineLegalPersonReadModel = ukraineLegalPersonReadModel;
        }

        public int Activate(long entityId)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity, LegalPerson>())
            {
                var restoringLegalPerson = _legalPersonReadModel.GetLegalPerson(entityId);
                if (restoringLegalPerson.IsActive)
                {
                    throw new NotificationException(string.Format(BLResources.LegalPersonToRestoreIsAlreadyActive, restoringLegalPerson.LegalName));
                }

                if (!string.IsNullOrWhiteSpace(restoringLegalPerson.Inn))
                {
                    var dublicateLegalPerson = _finder.FindMany(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                                                && LegalPersonSpecs.LegalPersons.Find.ByInn(restoringLegalPerson.Inn))
                                                      .FirstOrDefault();

                    if (dublicateLegalPerson != null)
                    {
                        throw new NotificationException(string.Format(BLResources.ActivateLegalPersonError, dublicateLegalPerson.LegalName));
                    }
                }

                var egrpou = restoringLegalPerson.Within<UkraineLegalPersonPart>().GetPropertyValue(part => part.Egrpou);
                if (_ukraineLegalPersonReadModel.AreThereAnyActiveEgrpouDuplicates(entityId, egrpou))
                {
                    throw new NotificationException(GetEgrpouDuplicateMessage((LegalPersonType)restoringLegalPerson.LegalPersonTypeEnum));
                }


                var result = _legalPersonRepository.Activate(entityId);

                operationScope
                    .Updated<LegalPerson>(entityId)
                    .Complete();

                return result;
            }
        }

        private static string GetEgrpouDuplicateMessage(LegalPersonType modelLegalPersonType)
        {
            return modelLegalPersonType == LegalPersonType.LegalPerson
                       ? Resources.Server.Properties.BLResources.UkraineActivateLegalPersonErrorDueToEgrpouDuplicate
                       : Resources.Server.Properties.BLResources.UkraineActivateBusinessmanErrorDueToEgrpouDuplicate;
        }
    }
}