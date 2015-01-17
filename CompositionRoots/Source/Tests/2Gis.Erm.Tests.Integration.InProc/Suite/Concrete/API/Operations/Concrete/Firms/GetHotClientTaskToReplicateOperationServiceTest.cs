using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Firms
{
    public class GetHotClientTaskToReplicateOperationServiceTest : UseModelEntityTestBase<HotClientRequest>
    {
        private readonly IGetHotClientRequestOperationService _getHotClientTaskToReplicateOperationService;
        private readonly IMsCrmSettings _msCrmSettings;

        public GetHotClientTaskToReplicateOperationServiceTest(
            IAppropriateEntityProvider<HotClientRequest> appropriateEntityProvider,
            IGetHotClientRequestOperationService getHotClientTaskToReplicateOperationService,
            IMsCrmSettings msCrmSettings)
            : base(appropriateEntityProvider)
        {
            _getHotClientTaskToReplicateOperationService = getHotClientTaskToReplicateOperationService;
            _msCrmSettings = msCrmSettings;
        }

        protected override OrdinaryTestResult ExecuteWithModel(HotClientRequest modelEntity)
        {
            return Result.When(_getHotClientTaskToReplicateOperationService.GetHotClientTask(modelEntity.Id))
                         .Then(r => r.Should().NotBeNull());
        }
    }
}