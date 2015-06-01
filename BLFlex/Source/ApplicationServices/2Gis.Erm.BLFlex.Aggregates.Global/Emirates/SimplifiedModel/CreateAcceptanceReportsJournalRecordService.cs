using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.SimplifiedModel;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.SimplifiedModel
{
    public class CreateAcceptanceReportsJournalRecordService : ICreateAcceptanceReportsJournalRecordService
    {
        private readonly IRepository<AcceptanceReportsJournalRecord> _acceptanceReportsJournalRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreateAcceptanceReportsJournalRecordService(IRepository<AcceptanceReportsJournalRecord> acceptanceReportsJournalRepository,
                                                           IIdentityProvider identityProvider,
                                                           IOperationScopeFactory operationScopeFactory)
        {
            _acceptanceReportsJournalRepository = acceptanceReportsJournalRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(AcceptanceReportsJournalRecord acceptanceReportsJournalRepositoryRecord)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, AcceptanceReportsJournalRecord>())
            {
                _identityProvider.SetFor(acceptanceReportsJournalRepositoryRecord);
                _acceptanceReportsJournalRepository.Add(acceptanceReportsJournalRepositoryRecord);
                operationScope.Added<AcceptanceReportsJournalRecord>(acceptanceReportsJournalRepositoryRecord.Id);

                var count = _acceptanceReportsJournalRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}