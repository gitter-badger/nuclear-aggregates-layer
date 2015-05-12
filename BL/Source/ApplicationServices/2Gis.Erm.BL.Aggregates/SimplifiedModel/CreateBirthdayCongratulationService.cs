using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BL.Aggregates.SimplifiedModel
{
    public class CreateBirthdayCongratulationService : ICreateBirthdayCongratulationService
    {
        private readonly IRepository<BirthdayCongratulation> _birthdayCongratulationRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IIdentityProvider _identityProvider;

        public CreateBirthdayCongratulationService(IRepository<BirthdayCongratulation> birthdayCongratulationRepository,
                                                   IOperationScopeFactory operationScopeFactory,
                                                   IIdentityProvider identityProvider)
        {
            _birthdayCongratulationRepository = birthdayCongratulationRepository;
            _operationScopeFactory = operationScopeFactory;
            _identityProvider = identityProvider;
        }

        public int Create(BirthdayCongratulation birthdayCongratulation)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, BirthdayCongratulation>())
            {
                _identityProvider.SetFor(birthdayCongratulation);
                _birthdayCongratulationRepository.Add(birthdayCongratulation);
                operationScope.Added<BirthdayCongratulation>(birthdayCongratulation.Id);

                var count = _birthdayCongratulationRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}