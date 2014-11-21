using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LocalMessages
{
    public class SaveLocalMessageHandlerTest : UseModelEntityHandlerTestBase<LocalMessage, SaveLocalMessageRequest, StreamResponse>
    {
        public SaveLocalMessageHandlerTest(IPublicService publicService, IAppropriateEntityProvider<LocalMessage> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(LocalMessage modelEntity, out SaveLocalMessageRequest request)
        {
            request = new SaveLocalMessageRequest
                {
                    Ids = new[] { modelEntity.Id }
                };

            return true;
        }
    }
}