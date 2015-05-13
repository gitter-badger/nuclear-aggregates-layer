using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLCore.Operations.Concrete.Old.Integration.ServiceBus
{
    public class SerializeAccountDetailHandlerTest 
        : UseModelEntityHandlerTestBase<AccountDetail, SerializeObjectsRequest<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial>, SerializeObjectsResponse>
    {
        private readonly IFinder _finder;

        public SerializeAccountDetailHandlerTest(IPublicService publicService, IAppropriateEntityProvider<AccountDetail> appropriateEntityProvider, IFinder finder) 
            : base(publicService, appropriateEntityProvider)
        {
            _finder = finder;
        }

        protected override IResponseAsserter<SerializeObjectsResponse> ResponseAsserter
        {
            get { return new SerializeObjectsResponseAsserter(); }
        }

        protected override bool TryCreateRequest(AccountDetail modelEntity, out SerializeObjectsRequest<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial> request)
        {
            var withdrawOperation =
                _finder.Find<PerformedBusinessOperation>(operation => operation.Operation == WithdrawFromAccountsIdentity.Instance.Id)
                       .OrderByDescending(operation => operation.Date)
                       .FirstOrDefault();

            var revertWithdrawOperation =
                _finder.Find<PerformedBusinessOperation>(operation => operation.Operation == RevertWithdrawFromAccountsIdentity.Instance.Id)
                       .OrderByDescending(operation => operation.Date)
                       .FirstOrDefault();

            if (withdrawOperation == null || revertWithdrawOperation == null)
            {
                request = null;
                return false;
            }

            request = SerializeObjectsRequest<AccountDetail, ExportFlowFinancialDataDebitsInfoInitial>.Create(
                "flowFinancialData_DebitsInfoInitial",
                new[] { withdrawOperation, revertWithdrawOperation });
            return true;
        }

        public class SerializeObjectsResponseAsserter : IResponseAsserter<SerializeObjectsResponse>
        {
            public OrdinaryTestResult Assert(SerializeObjectsResponse result)
            {
                result.FailedObjects.Should().BeEmpty();
                result.SerializedObjects.Should().HaveCount(2);
                return OrdinaryTestResult.As.Succeeded;
            }
        }
    }
}