using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Clients
{
    public class CreateHotClientHandlerTest : UseModelEntityHandlerTestBase<HotClientRequest, CreateHotClientRequest, CreateHotClientResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;

        public CreateHotClientHandlerTest(IPublicService publicService,
                                          IAppropriateEntityProvider<HotClientRequest> appropriateEntityProvider,
                                          IMsCrmSettings msCrmSettings)
            : base(publicService, appropriateEntityProvider)
        {
            _msCrmSettings = msCrmSettings;
        }

        protected override FindSpecification<HotClientRequest> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<HotClientRequest>(hcr => hcr.TaskId == null); }
        }

        protected override bool TryCreateRequest(HotClientRequest modelEntity, out CreateHotClientRequest request)
        {
            if (!_msCrmSettings.EnableReplication)
            {
                request = null;
                return false;
            }

            request = new CreateHotClientRequest
                {
                    Id = modelEntity.Id
                };

            return true;
        }


        protected override IResponseAsserter<CreateHotClientResponse> ResponseAsserter
        {
            get { return new DelegateResponseAsserter<CreateHotClientResponse>(Assert); }
        }

        private OrdinaryTestResult Assert(CreateHotClientResponse response)
        {
            return Result.When(response).Then(r => r.Success.Should().BeTrue());
        }
    }
}