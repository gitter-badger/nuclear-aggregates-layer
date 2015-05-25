using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateBargainOperationService : IActivateGenericEntityService<Bargain>
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IActivateAggregateRepository<Bargain> _activateAggregateService;
        private readonly IOrderReadModel _orderReadModel;

        public ActivateBargainOperationService(IOperationScopeFactory scopeFactory,
                                               IOrderReadModel orderReadModel,
                                               IActivateAggregateRepository<Bargain> activateAggregateService)
        {
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
            _activateAggregateService = activateAggregateService;
        }

        public int Activate(Bargain bargain)
        {
            if (bargain.BargainKind == BargainKind.Agent)
            {
                var duplicateNumber = _orderReadModel.GetDuplicateAgentBargainNumber(bargain.Id,
                                                                                     bargain.CustomerLegalPersonId,
                                                                                     bargain.ExecutorBranchOfficeId,
                                                                                     bargain.SignedOn,
                                                                                     bargain.BargainEndDate.Value);
                if (duplicateNumber != null)
                {
                    throw new AgentBargainIsNotUniqueException(string.Format(BLResources.AgentBargainIsNotUnique, duplicateNumber));
                }
            }

            // COMMENT {d.ivanov, 10.07.2014}: И всё-таки, я не понимаю, зачем дважды логировать одну и ту же операцию - с одними и теми же параметрами, одна родительская, другая вложенная.
            using (var scope = _scopeFactory.CreateSpecificFor<ActivateIdentity, Bargain>())
            {
                var count = _activateAggregateService.Activate(bargain.Id);

                scope.Updated<Bargain>(bargain.Id)
                     .Complete();

                return count;
            }
        }

        public int Activate(long entityId)
        {
            return Activate(_orderReadModel.GetBargain(entityId));
        }
    }
}