using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Limits
{
    public class CheckLimitLockedByReleaseHandlerTest : UseModelEntityTestBase<Limit>
    {
        private readonly IPublicService _publicService;

        public CheckLimitLockedByReleaseHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Limit> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _publicService = publicService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Limit modelEntity)
        {
            try
            {
                _publicService.Handle(new CheckLimitLockedByReleaseRequest { Entity = modelEntity });
            }

            catch (NotificationException)
            {
            }

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}