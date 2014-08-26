using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Activate
{
    public sealed class ChileActivateLegalPersonService : IActivateGenericEntityService<LegalPerson>, IChileAdapted
    {
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChileActivateLegalPersonService(
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory,
            ILegalPersonReadModel legalPersonReadModel)
        {
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
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
                    var duplicateLegalPersonName = _legalPersonReadModel.GetActiveLegalPersonNameWithSpecifiedInn(restoringLegalPerson.Inn);

                    if (duplicateLegalPersonName != null)
                   {
                        throw new NotificationException(string.Format(BLResources.ActivateLegalPersonError, duplicateLegalPersonName));
                   }
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