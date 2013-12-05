using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Firms;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Firms
{
    public class FirmAdditionalServiceOperationsTest : IIntegrationTest
    {
        private readonly IFirmAdditionalServiceOperations _firmAdditionalServiceOperations;
        private readonly IAppropriateEntityProvider<Firm> _firmEntityProvider;
        private readonly IAppropriateEntityProvider<FirmAddress> _firmAddressEntityProvider;

        public FirmAdditionalServiceOperationsTest(IFirmAdditionalServiceOperations firmAdditionalServiceOperations,
                                                   IAppropriateEntityProvider<Firm> firmEntityProvider,
                                                   IAppropriateEntityProvider<FirmAddress> firmAddressEntityProvider)
        {
            _firmAdditionalServiceOperations = firmAdditionalServiceOperations;
            _firmEntityProvider = firmEntityProvider;
            _firmAddressEntityProvider = firmAddressEntityProvider;
        }

        public ITestResult Execute()
        {
            var firmWithServices = _firmEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Firm>() &&
                                               new FindSpecification<Firm>(
                                                   f => f.FirmAddresses.Any(fa => fa.IsActive && !fa.IsDeleted && fa.FirmAddressServices.Any())));

            var firmAddressWithServices = _firmAddressEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<FirmAddress>() &&
                                                             new FindSpecification<FirmAddress>(
                                                                 fa => fa.IsActive && !fa.IsDeleted && fa.FirmAddressServices.Any()));

            var firm = _firmEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Firm>());
            var firmAddress = _firmAddressEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<FirmAddress>());

            if (firmWithServices == null || firmAddressWithServices == null)
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var firmAdditionalServices = _firmAdditionalServiceOperations.GetFirmAdditionalServices(firm.Id).ToArray();
            var firmAddressAdditionalServices = _firmAdditionalServiceOperations.GetFirmAddressAdditionalServices(firmAddress.Id).ToArray();


            if (!firmAdditionalServices.Any() || !firmAddressAdditionalServices.Any())
            {
                return OrdinaryTestResult.As.Failed;
            }

            _firmAdditionalServiceOperations.SetFirmServices(firm.Id, firmAdditionalServices);
            _firmAdditionalServiceOperations.SetFirmAddressServices(firm.Id, firmAddressAdditionalServices);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}