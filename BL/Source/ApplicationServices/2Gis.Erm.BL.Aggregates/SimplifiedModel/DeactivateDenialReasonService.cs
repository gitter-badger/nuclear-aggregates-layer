using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Aggregates.SimplifiedModel
{
    public class DeactivateDenialReasonService : IDeactivateDenialReasonService
    {
        private readonly IRepository<DenialReason> _denialReasonRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeactivateDenialReasonService(IRepository<DenialReason> denialReason,
                                             IOperationScopeFactory operationScopeFactory)
        {
            _denialReasonRepository = denialReason;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Deactivate(DenialReason denialReason)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, DenialReason>())
            {
                denialReason.IsActive = false;
                _denialReasonRepository.Update(denialReason);
                operationScope.Updated<DenialReason>(denialReason.Id);

                var count = _denialReasonRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}