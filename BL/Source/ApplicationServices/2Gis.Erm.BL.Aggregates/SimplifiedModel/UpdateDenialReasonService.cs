using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BL.Aggregates.SimplifiedModel
{
    public class UpdateDenialReasonService : IUpdateDenialReasonService
    {
        private readonly IRepository<DenialReason> _denialReasonRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public UpdateDenialReasonService(IRepository<DenialReason> denialReason,
                                         IOperationScopeFactory operationScopeFactory)
        {
            _denialReasonRepository = denialReason;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Update(DenialReason denialReason)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DenialReason>())
            {
                _denialReasonRepository.Update(denialReason);
                operationScope.Updated<DenialReason>(denialReason.Id);

                var count = _denialReasonRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}