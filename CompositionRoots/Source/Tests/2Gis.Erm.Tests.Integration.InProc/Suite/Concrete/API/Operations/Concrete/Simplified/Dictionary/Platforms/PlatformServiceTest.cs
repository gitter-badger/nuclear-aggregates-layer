using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Platforms
{
    public class PlatformServiceTest : UseModelEntityTestBase<Erm.Platform.Model.Entities.Erm.Platform>
    {
        private readonly IPlatformService _platformService;

        public PlatformServiceTest(IAppropriateEntityProvider<Erm.Platform.Model.Entities.Erm.Platform> appropriateEntityProvider, IPlatformService platformService)
            : base(appropriateEntityProvider)
        {
            _platformService = platformService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Erm.Platform.Model.Entities.Erm.Platform modelEntity)
        {
            modelEntity.MinPlacementPeriodEnum = PositionPlatformMinPlacementPeriod.FourMonths;
            _platformService.CreateOrUpdate(modelEntity);

            _platformService.GetPlatform(modelEntity.Id)
                .Should().NotBeNull();

            _platformService.GetPlatformWithPositions(modelEntity.Id)
                .Should().NotBeNull();

            _platformService.IsPlatformLinked(modelEntity.Id);

            var newPlatform = new Erm.Platform.Model.Entities.Erm.Platform
                {
                    Id = 7777777,
                    DgppId = 8888888,
                    Name = "testName"
                };
            _platformService.CreateOrUpdate(newPlatform);
            _platformService.Delete(newPlatform);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}