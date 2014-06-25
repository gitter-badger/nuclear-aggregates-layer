using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Aggregates.SimplifiedModel
{
    public class CreateDenialReasonService : ICreateDenialReasonService
    {
        private readonly IRepository<DenialReason> _denialReasonRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreateDenialReasonService(IRepository<DenialReason> denialReason,
                                         IOperationScopeFactory operationScopeFactory)
        {
            _denialReasonRepository = denialReason;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(DenialReason denialReason)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DenialReason>())
            {
                _denialReasonRepository.Add(denialReason);
                operationScope.Added<DenialReason>(denialReason.Id);

                var count = _denialReasonRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}