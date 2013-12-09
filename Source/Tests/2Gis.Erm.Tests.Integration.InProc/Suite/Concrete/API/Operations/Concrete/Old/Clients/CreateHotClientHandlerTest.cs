using DoubleGis.Erm.BL.API.Operations.Concrete.Old.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Clients
{
    public class CreateHotClientHandlerTest : UseModelEntityHandlerTestBase<HotClientRequest, CreateHotClientRequest, CreateHotClientResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;

        public CreateHotClientHandlerTest(IPublicService publicService, IAppropriateEntityProvider<HotClientRequest> appropriateEntityProvider, IMsCrmSettings msCrmSettings)
            : base(publicService, appropriateEntityProvider)
        {
            _msCrmSettings = msCrmSettings;
        }

        protected override FindSpecification<HotClientRequest> ModelEntitySpec
        {
            get { return new FindSpecification<HotClientRequest>(hcr => hcr.TaskId == null); }
        }

        protected override bool TryCreateRequest(HotClientRequest modelEntity, out CreateHotClientRequest request)
        {
            request = new CreateHotClientRequest
                {
                    Id = modelEntity.Id
                };

            return true;
        }

        protected override OrdinaryTestResult AssertResponse(CreateHotClientResponse response)
        {
            return _msCrmSettings.EnableReplication
                       ? Result.When(response).Then(r => r.Success.Should().BeTrue())
                       : base.AssertResponse(response);
        }
    }
}