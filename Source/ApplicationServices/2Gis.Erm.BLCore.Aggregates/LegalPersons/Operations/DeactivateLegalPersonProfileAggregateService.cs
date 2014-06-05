using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    // TODO {all, 05.06.2014}: Лучше бы помечать подобных чуваков интерфейсом IAggregateSpecificOperation.
    //                         С другой стороны, это ограничивает перечень возможных операций с persistence только CUD,
    //                         поэтому для полноценного перехода нужно менять контракт I%Operation%AggregateRepository и принимать не id, а сущность;
    //                         а в generic-сервисах получать ее через read model.
    public class DeactivateLegalPersonProfileAggregateService : IAggregatePartRepository<LegalPerson>, IDeactivateAggregateRepository<LegalPersonProfile>
    {
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureFinder _secureFinder;

        public DeactivateLegalPersonProfileAggregateService(ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository,
                                                            IOperationScopeFactory operationScopeFactory,
                                                            ISecureFinder secureFinder)
        {
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
            _operationScopeFactory = operationScopeFactory;
            _secureFinder = secureFinder;
        }

        int IDeactivateAggregateRepository<LegalPersonProfile>.Deactivate(long entityId)
        {
            return Deactivate(_secureFinder.Find(Specs.Find.ById<LegalPersonProfile>(entityId)).Single());
        }

        private int Deactivate(LegalPersonProfile entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, LegalPersonProfile>())
            {
                entity.IsActive = false;
                _legalPersonProfileSecureRepository.Update(entity);

                var count = _legalPersonProfileSecureRepository.Save();

                operationScope.Updated<LegalPersonProfile>(entity.Id)
                              .Complete();
                return count;
            }
        }
    }
}