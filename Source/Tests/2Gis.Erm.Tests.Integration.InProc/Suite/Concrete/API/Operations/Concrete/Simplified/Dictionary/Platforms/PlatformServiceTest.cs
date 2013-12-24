using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified.Dictionary.Platforms;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Platforms
{
    public class PlatformServiceTest : UseModelEntityTestBase<Platform.Model.Entities.Erm.Platform>
    {
        private readonly IPlatformService _platformService;

        public PlatformServiceTest(IAppropriateEntityProvider<Platform.Model.Entities.Erm.Platform> appropriateEntityProvider, IPlatformService platformService)
            : base(appropriateEntityProvider)
        {
            _platformService = platformService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Platform.Model.Entities.Erm.Platform modelEntity)
        {
            modelEntity.MinPlacementPeriodEnum = (int)PositionPlatformMinPlacementPeriod.FourMonths;
            _platformService.CreateOrUpdate(modelEntity);

            try
            {
                _platformService.Delete(modelEntity);
            }
            catch (NotificationException)
            {
            }

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}