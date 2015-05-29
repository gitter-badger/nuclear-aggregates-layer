using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class UpdateLegalPersonProfileAggregateService : IAggregateRootService<LegalPerson>, IUpdateAggregateRepository<LegalPersonProfile>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;

        public UpdateLegalPersonProfileAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
        }

        public int Update(LegalPersonProfile entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, LegalPersonProfile>())
            {
                _legalPersonProfileSecureRepository.Update(entity);
                operationScope.Updated<LegalPersonProfile>(entity.Id);

                var count = _legalPersonProfileSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}