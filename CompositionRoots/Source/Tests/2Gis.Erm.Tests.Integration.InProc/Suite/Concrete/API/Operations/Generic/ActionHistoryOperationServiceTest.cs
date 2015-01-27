using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class ActionHistoryOperationServiceTest : UseModelEntityTestBase<Client>
    {
        private readonly IActionsHistoryService _actionsHistoryOperationService;

        public ActionHistoryOperationServiceTest(
            IActionsHistoryService actionsHistoryOperationService,
            IAppropriateEntityProvider<Client> appropriateEntityProvider) 
            : base(appropriateEntityProvider)
        {
            _actionsHistoryOperationService = actionsHistoryOperationService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Client modelEntity)
        {
            return Result
                .When(_actionsHistoryOperationService.GetActionHistory(EntityType.Instance.Client(), modelEntity.Id))
                .Then(history => history.Should().NotBeNull());
        }
    }
}