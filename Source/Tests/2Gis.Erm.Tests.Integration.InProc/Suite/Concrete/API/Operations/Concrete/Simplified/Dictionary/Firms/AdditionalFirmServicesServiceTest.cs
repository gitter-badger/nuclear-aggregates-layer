using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Firms;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Firms
{
    public class AdditionalFirmServicesServiceTest : UseModelEntityTestBase<AdditionalFirmService>
    {
        private readonly IAdditionalFirmServicesService _additionalFirmServicesService;

        public AdditionalFirmServicesServiceTest(IAppropriateEntityProvider<AdditionalFirmService> appropriateEntityProvider,
                                                 IAdditionalFirmServicesService additionalFirmServicesService) : base(appropriateEntityProvider)
        {
            _additionalFirmServicesService = additionalFirmServicesService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(AdditionalFirmService modelEntity)
        {
            modelEntity.Description = "Test";
            _additionalFirmServicesService.CreateOrUpdate(modelEntity);

            try
            {
                _additionalFirmServicesService.Delete(modelEntity);
            }
            catch (NotificationException)
            {
            }

            return Result.When(modelEntity)
                         .Then(e => e.Id.Should().NotBe(0));
        }
    }
}