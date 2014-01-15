using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Limits
{
    public class CreateLimitHandlerTest : UseModelEntityHandlerTestBase<Account, CreateLimitRequest, CreateLimitResponse>
    {
        public CreateLimitHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Account> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Account modelEntity, out CreateLimitRequest request)
        {
            request = new CreateLimitRequest
                {
                    AccountId = modelEntity.Id
                };

            return true;
        }

        protected override IResponseAsserter<CreateLimitResponse> ResponseAsserter
        {
            get { return new DelegateResponseAsserter<CreateLimitResponse>(Assert); }
        }

        private static OrdinaryTestResult Assert(CreateLimitResponse response)
        {
            return Result.When(response)
                         .Then(r => r.StartPeriodDate.Should().BeAfter(DateTime.UtcNow));
        }
    }
}