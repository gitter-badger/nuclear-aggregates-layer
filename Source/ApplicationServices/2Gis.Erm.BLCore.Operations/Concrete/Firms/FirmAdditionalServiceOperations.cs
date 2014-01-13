using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Firms
{
    // FIXME {all, 23.10.2013}: опять набор операций подвязан к одной identity - т.е. профанация операции связаны только сущностью над которой они выполняются, необходимо выделить конкретные операции SRP
    public class FirmAdditionalServiceOperations : IFirmAdditionalServiceOperations
    {
        private readonly IFirmRepository _firmRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public FirmAdditionalServiceOperations(IFirmRepository firmRepository, IOperationScopeFactory operationScopeFactory)
        {
            _firmRepository = firmRepository;
            _scopeFactory = operationScopeFactory;
        }

        public void SetFirmServices(long firmId, IEnumerable<AdditionalServicesDto> services)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<SpecifyAdditionalServicesIdentity>(EntityName.Firm))
            {
                _firmRepository.SetFirmAdditionalServices(firmId, services);

                operationScope.Updated<Firm>(firmId);
                operationScope.Complete();
            }
        }

        public void SetFirmAddressServices(long firmAddressId, IEnumerable<AdditionalServicesDto> services)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<SpecifyAdditionalServicesIdentity>(EntityName.FirmAddress))
            {
                _firmRepository.SetFirmAddressAdditionalServices(firmAddressId, services);

                operationScope
                    .Updated<FirmAddress>(firmAddressId)
                    .Complete();
            }
        }

        public IEnumerable<AdditionalServicesDto> GetFirmAdditionalServices(long firmId)
        {
            return _firmRepository.GetFirmAdditionalServices(firmId);
        }

        public IEnumerable<AdditionalServicesDto> GetFirmAddressAdditionalServices(long firmAddressId)
        {
            return _firmRepository.GetFirmAddressAdditionalServices(firmAddressId);
        }
    }
}
