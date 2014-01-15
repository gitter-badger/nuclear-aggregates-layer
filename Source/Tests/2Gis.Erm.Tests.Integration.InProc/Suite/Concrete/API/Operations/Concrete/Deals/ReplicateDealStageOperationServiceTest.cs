using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Deals
{
    public class ReplicateDealStageOperationServiceTest : UseModelEntityTestBase<Deal>
    {
        private readonly IReplicateDealStageOperationService _replicateDealStageOperationService;

        public ReplicateDealStageOperationServiceTest(IAppropriateEntityProvider<Deal> appropriateEntityProvider,
                                                      IReplicateDealStageOperationService replicateDealStageOperationService) : base(appropriateEntityProvider)
        {
            _replicateDealStageOperationService = replicateDealStageOperationService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Deal modelEntity)
        {
            return Result.When(() => _replicateDealStageOperationService.Replicate(modelEntity.ReplicationCode, DealStage.MatchAndSendProposition, null))
                         .Then(result => result.Succeeded.Should().BeTrue());
        }
    }
}